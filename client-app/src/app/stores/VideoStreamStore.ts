import { makeAutoObservable } from "mobx";
import { LocalVideoStream } from "@azure/communication-calling";

export default class VideoStreamStore {
    localVideoStream: LocalVideoStream | null = null;
    callEnded: boolean = false;

    constructor() {
        makeAutoObservable(this);
    }

    setLocalVideoStream(stream: LocalVideoStream | null) {
        this.localVideoStream = stream;
    }

    setCallEnded(ended: boolean) {
        this.callEnded = ended;
    }
}