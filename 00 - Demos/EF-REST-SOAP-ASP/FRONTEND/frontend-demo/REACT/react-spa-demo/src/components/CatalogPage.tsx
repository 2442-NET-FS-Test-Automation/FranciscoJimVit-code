import { useState } from "react";
import { BookCard } from "./BookCard";
import {catalog} from "../data/catalog.ts";

export function CatalogPage() {
    const [items] = useState(catalog);

    const [compact, setCompact] = useState(false);


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
}