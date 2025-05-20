import { Navigate } from "react-router-dom";
import { useStore } from "../Store";

interface PrivateRouteProps {
    children: JSX.Element;
}

function PrivateRoute({ children }: PrivateRouteProps) {
    const { profileStore } = useStore();

    if (profileStore.logedInUSer() === null) {
        return <Navigate to="/login" replace />;
    }
    return children;
}

export default PrivateRoute;
