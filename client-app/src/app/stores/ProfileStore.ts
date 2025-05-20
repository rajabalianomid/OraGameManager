import { makeAutoObservable } from "mobx";
import { LoginModel } from "../models/LoginModel";
import agent from "../features/api/agent";
import { UserModel } from "../models/UserModel";
import { store } from "../Store";
import { router } from "../router/Routers";

export default class ProfileStore {

    user: UserModel | null = null;

    constructor() {
        makeAutoObservable(this);
    }

    login = async (model: LoginModel) => {
        debugger;
        try {
            const user = await agent.Auth.login(model);
            this.user = user;
            store.commonStore.setToken(user.token);
            localStorage.setItem('user', JSON.stringify(user));
            router.navigate('/');
        }
        catch (error) {
            debugger;
            throw error;
        }
    }
    
    logout = async () => {
        this.user = null;
        store.commonStore.setToken(null);
        localStorage.removeItem("user");
        router.navigate("/login");
    };

    logedInUSer = () => {
        const user = localStorage.getItem('user');
        if (user) {
            this.user = JSON.parse(user) as UserModel;
            return this.user;
        }
        return null;
    }
}
