import { useEffect, useState } from "react";
import { data, Link, useParams } from "react-router-dom";
import { getInventoryItem } from "./inventory";
import type { InventoryItem, FetchState } from "../types";

export function BookDetail() {
    const {sku} = useParams<{sku: string}>();
    const {item, setItem} = useState<InventoryItem | null>(null);
    const [fstate, setFState] = useState<FetchState>("idle");

    useEffect(() => {
        if (!sku) return;

        let active = true;
        setFState("loading");

        getInventoryItem(sku)
            .then((data) => {
                if(!active) return;
                setItem(data);
                setFState("loaded");
            })
            .catch(() => {
                if(active) setFState("failed");
            });

        return () => {
            active = false;
        };

    }, [sku]);

    if (fstate === "idle" || fstate === "loading") return <p>Loading...</p>

    if (fstate === "failed" || !item)
        return(
            <p>
                Book {sku} not found. <Link to={"/"}>Back to Catalog</Link>
            </p>
        );

    return(
        <article>
            <p>
                <Link to={"/"}>&larr; Back to catalog</Link>
            </p>
            <h2>{item.name}</h2>
            <p>SKU: {item.sku}</p>
            <p>In Stock: {item.currentStock}</p>
        </article>
    )
}