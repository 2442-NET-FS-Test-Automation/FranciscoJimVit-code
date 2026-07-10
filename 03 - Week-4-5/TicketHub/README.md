# TicketHub — README Writeup

## 1. Domain & Order Scope
- **Domain:** Event Ticketing (`ConcertSeat`, `TicketStock` & `Booking`).
- **Order Scope:** **Single-line bookings** (MVP Simplification). Each generated order requests a quantity for a single specific ticket section.

---

## 2. Technique → Type/Endpoint Map

- **EF Core Model Configuration:** Defined via Data Annotations in entities like `ConcertSeat.cs` (`[Required]`, `[StringLength]`) and `TicketStock.cs` (`[Timestamp]`).
- **DbContext Thread Isolation:** Solved using `IDbContextFactory<TicketHubDbContext>` in `FulfillmentService.cs` to instantiate a fresh, independent context per asynchronous operation block.
- **Minimal API Endpoint Routing:** Implemented in `Program.cs` via `MapPost("/seed")`, `MapGet("/inventory")`, `MapPost("/orders/burst")`, `MapPost("/benchmark")`, and the `/reports/*` path suite.
- **Background Processing Offloading:** Achieved via `Task.Run` inside the `POST /orders/burst` handler in `Program.cs`.
- **Optimistic Concurrency Retry Loop:** Fully implemented inside `FulfillmentService.FulfillOneAsync` using a `try-catch` block for `DbUpdateConcurrencyException` and an exponential random *backoff* via `Task.Delay`.
- **Structured Logging:** Powered by **Serilog** templates (`Log.Warning("... {BookingId} ...", bookingId)`) throughout `Program.cs` and `FulfillmentService.cs`.

---

## 3. Big-O Algorithmic Analysis

- **Priority Queue:** *Not applicable / Pending implementation.* (Currently, the engine processes elements using a flat linear collection via `Parallel.ForEachAsync`).
- **Lookups:** *Not applicable / Pending implementation.* (Entity lookups are currently evaluated through Direct EF Core database queries instead of in-memory dictionary hashing).
- **Report Sort:** **$O(N \log N)$** average time complexity. Powered by LINQ's sorting architecture inside the `/reports/*` endpoints in `Program.cs` (e.g., `.OrderByDescending()`), which uses a highly efficient Timsort/Introsort hybrid to group and rank metrics.

---

## 4. The Token-vs-Lock Contrast

- **EF Core Optimistic Concurrency (RowVersion):** The application reads the inventory row without holding a persistent lock. Upon saving changes, SQL Server verifies if the `RowVersion` has changed. If another thread mutated the stock first, a `DbUpdateConcurrencyException` is thrown, triggering our application-level retry loop. This fits perfectly for scale-out cloud systems with low-to-medium collision windows.
- **In-Memory Locks (`lock` / `Interlocked`):** Restricts access to a code block or memory address to a single thread at a time. While highly efficient for in-memory operations within a single process node, it does not scale across multiple application server instances and can bottleneck database throughput under heavy multi-threaded stress.

---

## 5. ACID & Isolation Level Reasoning

Each single-line booking execution block runs within an independent database transaction executing under the **Read Committed** isolation level, satisfying all ACID properties:
- **Atomicity:** The reduction of `QuantityOnHand` in `TicketStock` and the insertion of the audited `FulfillmentEvent` are committed as a single unit. If the stock update fails, no records are saved.
- **Consistency:** Database state limits are tightly guarded. System checks guarantee that `QuantityOnHand` can never be driven below zero.
- **Isolation:** Using Read Committed combined with application-level Optimistic Concurrency tokens prevents concurrent operations from experiencing dirty reads or corrupting data states during simultaneous ráfagas.
- **Durability:** Once `SaveChangesAsync()` successfully returns, the inventory modification is securely pushed into the transaction logs on disk.

---

## 6. Non-Key Indexes Justification

- **`Customer.Email` (Unique Index):** Explicitly required to prevent transactional duplication and guarantee lookup speed when mapping distinct customers.
- **`ConcertSeat.Sku` (Unique Index):** Essential for performance optimization, turning slower $O(N)$ table scans into $O(1)$ or $O(\log N)$ index seeks when identifying products via string identifiers during data seeding and order processing loops.

---

## 7. Benchmark Numbers & Concurrency Note

- **Endpoint Implemented:** `POST /benchmark`
- **Execution Strategy:** Evaluates performance by seeding independent, equivalent order waves of size `N`, running the first batch through a sequential `foreach` loop and the second batch utilizing multi-threaded background task processing (`Parallel.ForEachAsync` inside `FulfillBurstAsync`).
- **Parallelism vs. Concurrency Note:** *Concurrency* is about execution structure—the ability to manage and orchestrate multiple tasks overlapping in time (such as handling incoming requests while a background thread works). *Parallelism* is about execution hardware—physically running multiple computations at the exact same physical instant utilizing multiple CPU cores simultaneously.

---

## 8. HTTP Status Codes Selection

The web surface exposes the following explicit status codes based on the Engineering Definition of Done:
- **`200 OK`:** Returned by `GET /inventory`, `GET /reports/*`, and `POST /benchmark` because the requested analytical computation or data state exists and successfully returns its payload.
- **`202 Accepted`:** Emitted immediately by `POST /orders/burst`. This indicates that the incoming payload is valid, and the processing workload has been safely offloaded onto a background worker thread.