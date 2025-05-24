import { makeAutoObservable, runInAction } from "mobx";
import { Pagination } from "../models/Pagination";
import { RoomModel } from "../models/RoomModel";
import agent from "../features/api/agent";
import { CurrentRoomModel } from "../models/CurrentRoomModel";

export default class RoomStore {

    currentRoom: CurrentRoomModel | null = null;
    rooms: Pagination<RoomModel> = {
        data: [],
        currentPage: 1,
        size: 10,
        total: 0,
    };
    loading: boolean = false;

    constructor() {
        makeAutoObservable(this);
    }

    loadRooms = async (page: number) => {
        this.loading = true;
        try {
            debugger;
            const result = await agent.Rooms.getRooms(page, this.rooms.size);
            runInAction(() => {
                this.rooms = {
                    ...this.rooms,
                    data: result.data,
                    currentPage: page,
                    total: result.count,
                };
            });
        } catch (error) {
            console.error("Failed to load rooms:", error);
        } finally {
            runInAction(() => { this.loading = false; });
        }
    };

    getCurrentRoom = () => {
        debugger;
        const room = localStorage.getItem('currentRoom');
        if (room) {
            this.currentRoom = { id: Number(room) } as CurrentRoomModel;
            return this.currentRoom;
        }
    };
}
