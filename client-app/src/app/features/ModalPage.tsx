import { observer } from "mobx-react-lite";
import { useStore } from "../Store";

export interface ModalPageProps {
    isOpen: boolean;
    title: string;
    content: React.ReactNode;
}

function ModalPage(props: ModalPageProps) {

    const { commonStore } = useStore();

    return (
        <div>
            {/* Modal */}
            <div
                className={`modal fade ${props.isOpen ? "show" : ""}`}
                id="modal-block-fadein"
                tabIndex={-1}
                aria-labelledby="modal-block-fadein"
                aria-hidden={!props.isOpen}
                style={{ display: props.isOpen ? "block" : "none" }}
                role="dialog"
            >
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="block block-rounded block-themed block-transparent mb-0">
                            <div className="block-header bg-primary-dark">
                                <h3 className="block-title">{props.title}</h3>
                                <div className="block-options">
                                    <button
                                        type="button"
                                        className="btn-block-option"
                                        onClick={commonStore.closeModal}
                                        aria-label="Close"
                                    >
                                        <i className="fa fa-fw fa-times"></i>
                                    </button>
                                </div>
                            </div>
                            <div className="block block-rounded block-themed text-center">
                                <div className="block-header bg-muted">
                                    <h3 className="block-title">Room Details</h3>
                                </div>
                                <div className="block-content">
                                    {props.content}
                                </div>
                            </div>
                            {/* <div className="block-content">
                                <p>{props.text}</p>
                            </div> */}
                            <div className="block-content block-content-full text-end bg-body">
                                <button
                                    type="button"
                                    className="btn btn-sm btn-alt-secondary"
                                    onClick={commonStore.closeModal}
                                >
                                    Close
                                </button>
                                <button
                                    type="button"
                                    className="btn btn-sm btn-primary"
                                    onClick={commonStore.closeModal}
                                >
                                    Done
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Backdrop */}
            {props.isOpen && <div className="modal-backdrop fade show"></div>}
        </div>
    );
}

export default observer(ModalPage);