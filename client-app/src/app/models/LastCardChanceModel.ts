export interface LastCardChanceModel {
    id: number;
    name: string;                    // required
    description?: string | null;    // optional and nullable
    icon?: string | null;           // optional and nullable
    selfAct: boolean;               // required
    showFront: boolean
}