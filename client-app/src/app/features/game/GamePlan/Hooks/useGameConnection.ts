// useGameConnection.ts
import { useEffect } from "react";
import { useStore } from "../../../../Store";
import agent from "../../../api/agent";
import { AppConfig } from "../../../../models/AppConfig";

export function useGameConnection(roomId?: string) {
    const { communicationStore, profileStore, mainStore } = useStore();
    const { initializeConnection, waitForConnection } = communicationStore;

    useEffect(() => {
        mainStore.setWithoutSlider(true);

        const setupConnection = async () => {
            const jwtToken = agent.getToken() || "";
            await initializeConnection(jwtToken);
            await waitForConnection();

            if (!roomId) return;

            const user = profileStore.logedInUSer();
            if (user?.userName) {
                await communicationStore.addUserToRoom(AppConfig.appId, roomId, user.userName);
            }
        };

        setupConnection();
    }, [initializeConnection, waitForConnection, roomId]);
}
