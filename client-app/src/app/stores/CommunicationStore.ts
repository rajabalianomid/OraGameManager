import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { makeAutoObservable, runInAction } from "mobx";
import { GameModel } from "../models/GameModel";
import { RoleModel } from "../models/RoleModel ";
import { MessageModel } from "../models/MessageModel";
import { TurnModel } from "../models/TurnModel";
import { ReportModel } from "../models/ReportModel";
import agent from "../features/api/agent";
import { videoCallToken } from "../models/VideoCallToken";
import { ActionModelResponse } from "../models/ActionModelResponse";

export default class CommunicationStore {

    gameModel: GameModel | null = null;
    connection: HubConnection | null = null;
    videoCallToken: videoCallToken = { userId: '', token: '', expiresOn: '' };
    onCall: boolean = false;
    turnModel: TurnModel | null = null;
    connectionState: string = "disconnected"; // Track connection state

    constructor() {
        makeAutoObservable(this);
    }

    // Initialize SignalR connection
    initializeConnection = async (jwtToken: string) => {
        if (this.connection?.state === "Connected" || this.connection?.state === "Connecting") {
            console.log("SignalR connection is already established or in progress.");
            return;
        }

        try {
            if (!this.connection) {
                this.connection = new HubConnectionBuilder()
                    .withUrl("https://localhost:5001/gamehub", {
                        accessTokenFactory: () => jwtToken,
                        withCredentials: true,
                    })
                    .withAutomaticReconnect()
                    .configureLogging(LogLevel.Information)
                    .build();

                this.connection.onreconnecting(() => {
                    console.warn("Reconnecting...");
                    runInAction(() => {
                        this.connectionState = "reconnecting";
                    });
                });

                this.connection.onreconnected(() => {
                    console.log("Reconnected!");
                    runInAction(() => {
                        this.connectionState = "connected";
                    });
                });

                this.connection.onclose(() => {
                    console.error("Connection closed");
                    runInAction(() => {
                        this.connectionState = "disconnected";
                    });
                });

                this.connection.on("UpdateGameModelForAllClientExceptCurrentOneInRoom", this.UpdateGameModelForAllClientExceptCurrentOneInRoom);
                this.connection.on("UpdateRoleModel", this.UpdateRoleModel);
                this.connection.on("TurnInfo", this.TurnInfo);
                this.connection.on("PlaySound", this.PlaySound);
                this.connection.on("ChallengeRequest", this.ChallengeRequest);
                this.connection.on("ChallengeRequestBack", this.ChallengeRequestBack);
                this.connection.on("ShowResultCard", this.ShowResultCard);
                this.connection.on("Report", this.Report);
                this.connection.on("ReciveMessage", this.ReciveMessage);
            }

            console.log("Starting SignalR connection...");
            await this.connection.start();
            runInAction(() => {
                this.connectionState = "connected";
            });
            console.log("SignalR Connected");
        } catch (error) {
            console.error("Error connecting to SignalR:");
            runInAction(() => {
                this.connectionState = "disconnected";
            });
        }
    };

    waitForConnection = async () => {
        if (!this.connection) {
            console.error("Connection is not initialized.");
            return;
        }

        while (this.connection.state !== "Connected") {
            console.log("Waiting for SignalR connection...");
            await new Promise((resolve) => setTimeout(resolve, 100)); // Wait 100ms
        }
    };
    disconnectConnection = async () => {
        if (this.connection) {
            console.log("Stopping SignalR connection...");
            await this.connection.stop();
            this.connectionState = "disconnected";
            console.log("SignalR Disconnected");
        }
    };

    UpdateGameModelForAllClientExceptCurrentOneInRoom = (model: GameModel) => {
        runInAction(() => {
            console.log('UpdateGameModelForAllClientExceptCurrentOneInRoom');
        });
    };
    UpdateRoleModel = (role: RoleModel) => {
        runInAction(() => {
            console.log('UpdateRoleModel');
        });
    };
    TurnInfo = (model: TurnModel) => {
        // debugger;
        runInAction(() => {
            this.turnModel = model;
            console.log('TurnInfo', this.turnModel);
        });
    };
    PlaySound = (soundName: string) => {
        runInAction(() => {
            console.log('PlaySound');
        });
    };
    ChallengeRequest = (userName: string) => {
        runInAction(() => {
            console.log('ChallengeRequest');
        });
    };
    ChallengeRequestBack = (userName: string) => {
        runInAction(() => {
            console.log('ChallengeRequestBack');
        });
    };
    ShowResultCard = (description: string) => {
        runInAction(() => {
            console.log('ShowResultCard');
        });
    };
    Report = (model: ReportModel) => {
        runInAction(() => {
            console.log('Report');
        });
    };
    ReciveMessage = (message: MessageModel) => {
        runInAction(() => {
            console.log('ReciveMessage');
        });
    };


    async addUserToRoom(appId: string, roomId: string, playerName: string): Promise<boolean> {
        try {
            if (!this.connection) {
                console.error("Connection is not established.");
                return false;
            }

            var canJoin = await this.connection.invoke<boolean>(
                "JoinRoomAuto",
                appId,
                roomId,
                playerName
            );
            if (!canJoin) {
                console.error("Failed to join room. Room may be full or does not exist.");
                return false;
            }
            localStorage.setItem('currentRoom', roomId);
            console.log("User added to room:", { appId, roomId, playerName });
        } catch (error) {
            console.error("Error adding user to room:", error);
        }
        return true;
    }

    async doAction(roomId: number, userId: string, hasGun: boolean) {
        debugger;
        if (roomId === 0 || userId === '')
            return;
        await this.connection?.invoke<ActionModelResponse>("DoAction", userId, Number(roomId), hasGun);
    }

    //#region Video Call

    getUser = async () => {
        debugger;
        try {
            const userToken = await agent.VideoCall.getUser();
            runInAction(() => { this.videoCallToken = userToken });
        }
        catch (error) {
            debugger;
            throw error;
        }
    }

    setOnCall(value: boolean) {
        this.onCall = value;
    }

    //#endregion
}