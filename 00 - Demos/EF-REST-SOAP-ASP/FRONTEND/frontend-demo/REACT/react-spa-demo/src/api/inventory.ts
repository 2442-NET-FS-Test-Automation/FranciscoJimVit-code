import { api } from "./client";
import type { InventoryItem } from "../types";

export async function getInventory(): Promise <InventoryItem[]> {
    const response = await api.get<InventoryItem[]>("/api/inventory");
    return response.data;
}

export async function getInventoryItem(sku:string): Promise<InventoryItem> {
    const response = await api.get<InventoryItem>(`/api/Inventory/${sku}`);
    return response.data;
}