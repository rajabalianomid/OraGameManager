import axios, { AxiosResponse } from "axios";
import { LoginModel } from "../../models/LoginModel";
import { Pagination } from "../../models/Pagination";
import { RoomModel } from "../../models/RoomModel";
import { videoCallToken } from "../../models/VideoCallToken";

axios.defaults.baseURL = 'https://localhost:7141';

const responseBody = (response: AxiosResponse) => response.data;

const getToken = () => localStorage.getItem("jwt");

axios.interceptors.request.use(config => {
    const token = getToken();
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});
axios.interceptors.response.use(
    response => response,
    error => {
        debugger;
        if (error.response?.status === 401) {
            // Redirect to login page on token expiration
            window.location.href = "/login";  // Direct navigation
        }
        return Promise.reject(error);
    }
);

const requests = {
    get: <T>(url: string) => axios.get<T>(url).then(responseBody),
    post: <T>(url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    del: <T>(url: string, body: {}) => axios.delete<T>(url, body).then(responseBody),
}

const Rooms = {
    getRooms: (page: number, size: number) => requests.post<Pagination<RoomModel>>('/api/room', { skip: (page - 1) * size, top: size, count: true, clientId: 1 })
    // allMembers: (id: number) => requests.get<Members[]>(`/api/v1/room/getallmember/${id}`),
    // joinRoom: (id: number) => requests.post(`/api/v1/room/join/${id}`, {}),
}
const VideoCall = {
    getUser: () => requests.post<videoCallToken>('/api/videocall/getuser_with_userid_and_token', () => { })
}
const Auth = {
    login: (credentials: LoginModel) => requests.post<any>('/api/login', credentials.form),
}

const agent = {
    Rooms,
    Auth,
    VideoCall,
    getToken
}

export default agent;