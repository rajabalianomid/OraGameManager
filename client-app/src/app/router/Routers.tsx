import { createBrowserRouter, RouteObject } from 'react-router-dom'
import App from '../layout/App'
import NotFound from '../features/errors/NotFound';
import Login from '../features/profile/Login';
import Room from '../features/game/Room';
import MainContainer from '../features/MainContainer';
import GamePlan from '../features/game/GamePlan';
import VideoCall from '../features/voip/VideoCall';
import PrivateRoute from './PrivateRoute';

export const routes: RouteObject[] = [
    {
        path: "/",
        element: <App />,
        children: [
            { index: true, element: (<PrivateRoute><MainContainer /></PrivateRoute>) },
            { path: '/index', element: (<PrivateRoute><MainContainer /></PrivateRoute>) },
            { path: '/room', element: (<PrivateRoute><Room /></PrivateRoute>) },
            { path: '/gameplan/:roomId', element: (<PrivateRoute><GamePlan /></PrivateRoute>) },
            { path: '/videocall', element: (<PrivateRoute><VideoCall /></PrivateRoute>) },
            { path: 'not-found', element: (<PrivateRoute><NotFound /></PrivateRoute>) },
            { path: '*', element: (<PrivateRoute><NotFound /></PrivateRoute>) }
        ]
    },
    {
        path: "/login",
        element: <Login />
    }
]

export const router = createBrowserRouter(routes);