import { makeAutoObservable, reaction } from "mobx";
import { ModalPageProps } from "../features/ModalPage";

export default class CommonStore {
    token: string | null = localStorage.getItem('jwt');
    modalPage: ModalPageProps = { isOpen: false, content: '', title: '' };

    constructor() {
        makeAutoObservable(this);

        reaction(() => this.token, token => {
            if (token) {
                localStorage.setItem('jwt', token);
            }
            else {
                localStorage.removeItem('jwt');
            }
        });
    }

    setToken = (token: string | null) => {
        this.token = token;
    }

    openModal = (title: string, content: React.ReactNode) => {
        this.modalPage.isOpen = true;
        this.modalPage.content = content;
        this.modalPage.title = title;
    };

    closeModal = () => {
        this.modalPage.isOpen = false;
        this.modalPage.content = '';
        this.modalPage.title = '';
    };
}