import { useEffect, useState } from "react";
import { BookCard } from "./BookCard";
// import {catalog} from "../data/catalog.ts";
import { SortDirection, type FetchState, type InventoryItem } from "../types.ts";
import { getInventory } from "../api/inventory.ts";
import { SearchBar } from "./SearchBar.tsx";

export function CatalogPage() {
    
    // const [compact, setCompact] = useState(false);
    
    const [items, setItems] = useState<InventoryItem[]>([]);
    const [fstate, setFState] = useState<FetchState>("idle");

    const [userQuery, setUserQuery] = useState("");
    const [dir, setDir] = useState<SortDirection>(SortDirection.Ascending);

    useEffect(() => {
        let active = true;

        setFState("loading");

        getInventory()
            .then( (data) => {
                if(!active) return;
                setItems(data);
                setFState("loaded");
            })
            .catch( () => {
                if(active) setFState("failed");
            });


            return() => {
                active = false;
            };
    }, [])
    
    const visibleBooks = [...items]
        .filter((i) => i.name.toLowerCase().includes(userQuery.toLowerCase()))
        .sort((a,b) => 
            dir === SortDirection.Ascending
                ? a.name.localeCompare(b.name)
                : b.name.localeCompare(a.name)
        );

    if (fstate === "idle" || fstate === "loading") return <p>Loading catalog...</p>

    if (fstate === "failed")
        return <p>Could not reach the API. Is it running on :5157? Check CORS.</p>

    return (
        <section>
            <div className="toolbar">
                <h2>Catalog</h2>
                <SearchBar value={userQuery} onChange={setUserQuery} />
                <button
                    type="button"
                    onClick={() => 
                        setDir((d) => 
                            d === SortDirection.Ascending
                            ? SortDirection.Descending
                            : SortDirection.Ascending,
                        )
                    }
                >
                    Sort {dir === SortDirection.Ascending ? "Z-A" : "A-Z"}
                </button>
            </div>

            {visibleBooks.length === 0? (
                <p>No books match "{userQuery}."</p>
            ) : (
                <div className="cards">
                    {visibleBooks.map((item) => (
                        <BookCard key={item.sku} item={item} />
                    ))}
                </div>
            )}
        </section>
)
    /* 
    return(

        <>
            <div className="toolbar">
                <h2>Catalog</h2>
                <button type="button" onClick={() => setCompact((c) => !c)}>
                    {compact ? "Show detail" : "Compact view"}
                </button>
            </div>

            <div className="cards">
                {
                    items.map((item) => (
                        <BookCard key={item.sku} item={item} compact = {compact}/>
                    ))
                }
            </div>
        </>
    ) 
    */
}