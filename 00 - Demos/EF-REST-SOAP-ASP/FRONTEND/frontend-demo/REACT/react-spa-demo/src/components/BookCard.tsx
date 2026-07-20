import type { InvalidEvent, InventoryItem } from "../types";

interface BookCardProps {
    item: InventoryItem;
    compact?: boolean;
}

export function BookCard ( {item, compact = false} : BookCardProps) {
    
    
    return (
        <article className="card">
            <h3> {item.name}</h3>
            <dl>
                <dt>SKU</dt>
                <dd>{item.sku}</dd>
                {!compact && (
                    <>
                        <dt>In Stock</dt>
                        <dd className={item.currentStock === 0 ? "out" : ""}>
                            {item.currentStock}
                        </dd>
                    </>
                )}
            </dl>
        </article>
    )
}