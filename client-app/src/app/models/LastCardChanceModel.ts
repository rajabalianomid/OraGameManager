export interface LastCardChanceModel {
    name: string;                    // required
    description?: string | null;    // optional and nullable
    icon?: string | null;           // optional and nullable
    selfAct: boolean;               // required
}