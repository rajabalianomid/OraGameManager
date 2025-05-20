import { useEffect } from "react";
import { observer } from "mobx-react-lite";
import { useStore } from "../../Store";
import ModalPage from "../ModalPage";
import { toJS } from "mobx";
import { router } from "../../router/Routers";

function Room() {

    const { roomStore, commonStore } = useStore();

    useEffect(() => {
        // Load the initial page
        roomStore.loadRooms(roomStore.rooms.currentPage);
    }, []); // Removed rooms.currentPage from dependency array

    const handleNextPage = () => {
        if (roomStore.rooms.currentPage < Math.ceil(roomStore.rooms.total / roomStore.rooms.size)) {
            roomStore.rooms.currentPage += 1; // MobX will handle reactivity
            roomStore.loadRooms(roomStore.rooms.currentPage);
        }
    };

    const handlePreviousPage = () => {
        if (roomStore.rooms.currentPage > 1) {
            roomStore.rooms.currentPage -= 1; // MobX will handle reactivity
            roomStore.loadRooms(roomStore.rooms.currentPage);
        }
    };

    const handleJoinRoom = (roomId: string) => {
        router.navigate('/gameplan/' + roomId);
    }

    if (roomStore.loading) return <p>Loading...</p>;

    console.log("Rooms after initial load:", toJS(roomStore.rooms));
    return (
        <main id="main-container">
            <div className="bg-body-light">
                <div className="content content-full">
                    <div className="d-flex flex-column flex-sm-row justify-content-sm-between align-items-sm-center">
                        <h1 className="flex-grow-1 fs-3 fw-semibold my-2 my-sm-3">Rooms</h1>
                        <nav className="flex-shrink-0 my-2 my-sm-0 ms-sm-3" aria-label="breadcrumb">
                            <ol className="breadcrumb">
                                <li className="breadcrumb-item">Home</li>
                                <li className="breadcrumb-item active" aria-current="page">Rooms</li>
                            </ol>
                        </nav>
                    </div>
                </div>
            </div>
            <div className="content">
                <div className="block block-rounded">
                    <div className="block-header block-header-default">
                        <h3 className="block-title">
                            List of rooms
                        </h3>
                    </div>
                    <div className="block-content block-content-full overflow-x-auto">
                        <table className="table table-bordered table-striped table-vcenter js-dataTable-full">
                            <thead>
                                <tr>
                                    <th className="text-center" style={{ width: '80px' }}>ID</th>
                                    <th style={{ width: '20%' }}>Name</th>
                                    <th className="d-none d-sm-table-cell">Description</th>
                                    <th style={{ width: '20%' }}></th>
                                </tr>
                            </thead>
                            <tbody>
                                {
                                    roomStore.rooms.data.map((room) => (
                                        <tr key={room.id}>
                                            <td className="text-center">{room.id}</td>
                                            <td className="fw-semibold"><a href="#" onClick={() => handleJoinRoom(room.id.toString())}>{room.name}</a></td>
                                            <td className="d-none d-sm-table-cell">
                                                <span className="text-muted">{room.description}</span>
                                            </td>
                                            <td className="text-center">
                                                <div className="btn-group">
                                                    <button type="button" className="btn btn-sm btn-alt-secondary js-bs-tooltip-enabled" data-bs-toggle="tooltip" aria-label="Show detail" data-bs-original-title="Show detail" onClick={() => commonStore.openModal(room.name,
                                                        <div className="py-2">                                                            <p>
                                                            <strong>Max Players:</strong> {room.maxPlayer}
                                                        </p>
                                                            <p>
                                                                <strong>Can GodFather Show By Detective:</strong>{" "}
                                                                {room.canGodFatherShowByDetective ? "Yes" : "No"}
                                                            </p>
                                                            <p>
                                                                <strong>Is Challenge:</strong>{" "}
                                                                {room.isChallenge ? "Yes" : "No"}
                                                            </p>
                                                            <p>
                                                                <strong>Challenge Time:</strong> {room.challengeTime} seconds
                                                            </p>
                                                            <p>
                                                                <strong>Defensed Time:</strong> {room.defensedTime} seconds
                                                            </p>
                                                            <p>
                                                                <strong>Action Time:</strong> {room.actionTime} seconds
                                                            </p>
                                                            <p>
                                                                <strong>Final Vote Time:</strong> {room.finalVoteTime} seconds
                                                            </p>
                                                            <p>
                                                                <strong>First Vote Time:</strong> {room.firstVoteTime} seconds
                                                            </p>
                                                            <p>
                                                                <strong>Speak Time:</strong> {room.speakTime} seconds
                                                            </p>
                                                            <p>
                                                                <strong>Expire Time:</strong> {new Intl.DateTimeFormat('en-US', {
                                                                    year: 'numeric',
                                                                    month: 'long',
                                                                    day: 'numeric',
                                                                    hour: '2-digit',
                                                                    minute: '2-digit',
                                                                }).format(new Date(room.expireTime))}
                                                            </p>
                                                            <p>
                                                                <strong>Description:</strong> {room.description}
                                                            </p>
                                                        </div>
                                                    )}>
                                                        <i className="fa fa-pencil-alt"></i>
                                                    </button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))}
                            </tbody>
                        </table>
                        <nav aria-label="Page navigation">
                            <ul className="pagination">
                                <li className={`page-item ${roomStore.rooms.currentPage === 1 ? "disabled" : ""}`}>
                                    <button className="page-link" onClick={handlePreviousPage} aria-label="Previous">
                                        <span aria-hidden="true">
                                            <i className="fa fa-angle-double-left"></i>
                                        </span>
                                        <span className="visually-hidden">Previous</span>
                                    </button>
                                </li>
                                <li className={`page-item ${roomStore.rooms.currentPage === Math.ceil(roomStore.rooms.total / roomStore.rooms.size) ? "disabled" : ""}`}>
                                    <button className="page-link" onClick={handleNextPage} aria-label="Next">
                                        <span aria-hidden="true">
                                            <i className="fa fa-angle-double-right"></i>
                                        </span>
                                        <span className="visually-hidden">Next</span>
                                    </button>
                                </li>
                            </ul>
                        </nav>
                    </div>
                </div>
            </div>
            <ModalPage isOpen={commonStore.modalPage.isOpen} title={commonStore.modalPage.title} content={commonStore.modalPage.content} />
        </main>
    );
}

export default observer(Room);