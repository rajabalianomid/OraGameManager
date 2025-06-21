import { createContext, useContext } from "react";
import MainStore from "./stores/MainStore";
import CommonStore from "./stores/CommonStore";
import ProfileStore from "./stores/ProfileStore";
import RoomStore from "./stores/RoomStore";
import CommunicationStore from "./stores/CommunicationStore";
import VideoStreamStore from "./stores/VideoStreamStore";

interface Store {
    mainStore: MainStore;
    commonStore: CommonStore;
    profileStore: ProfileStore;
    roomStore: RoomStore;
    communicationStore: CommunicationStore;
    videoStreamStore: VideoStreamStore;
}
export const store: Store = {
    mainStore: new MainStore(),
    commonStore: new CommonStore(),
    profileStore: new ProfileStore(),
    roomStore: new RoomStore(),
    communicationStore: new CommunicationStore(),
    videoStreamStore: new VideoStreamStore(),
}

export const StoreContext = createContext(store);

export function useStore() {
    return useContext(StoreContext);
}