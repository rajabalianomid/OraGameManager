import { makeAutoObservable } from "mobx";

export default class MainStore {

    isMenuClose: boolean = false;
    withOutSlider: boolean = false;

    constructor() {
        makeAutoObservable(this);
    }

    setMenuClose(state: boolean) {
        this.isMenuClose = state; // State modification happens inside an action
    }

    sidebarToggle = async () => {
        this.isMenuClose = !this.isMenuClose;
    }

    setWithoutSlider(state: boolean) {
        this.withOutSlider = state;
    }
}