import { observer } from "mobx-react-lite";
import { useStore } from "../Store";

function PageHeader() {

    const { mainStore } = useStore();

    return (
        <header id="page-header">
            <div className="content-header">
                <div className="space-x-1">
                    {
                        !mainStore.withOutSlider &&
                        (<button type="button" className="btn btn-alt-secondary" data-toggle="layout" data-action="sidebar_toggle" onClick={mainStore.sidebarToggle}>
                            <i className="fa fa-fw fa-bars"></i>
                        </button>)
                    }
                </div>
                <div className="space-x-1">
                    <div className="dropdown d-inline-block">
                        <button type="button" className="btn btn-alt-secondary" id="page-header-user-dropdown" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            <i className="fa fa-fw fa-user d-sm-none"></i>
                            <span className="d-none d-sm-inline-block">Admin</span>
                            <i className="fa fa-fw fa-angle-down opacity-50 ms-1 d-none d-sm-inline-block"></i>
                        </button>
                        <div className="dropdown-menu dropdown-menu-end p-0" aria-labelledby="page-header-user-dropdown">
                            <div className="bg-primary-dark rounded-top fw-semibold text-white text-center p-3">
                                User Options
                            </div>
                            <div className="p-2">
                                <a className="dropdown-item" href="be_pages_generic_profile.html">
                                    <i className="far fa-fw fa-user me-1"></i> Profile
                                </a>
                                <a className="dropdown-item d-flex align-items-center justify-content-between" href="be_pages_generic_inbox.html">
                                    <span><i className="far fa-fw fa-envelope me-1"></i> Inbox</span>
                                    <span className="badge bg-primary rounded-pill">3</span>
                                </a>
                                <a className="dropdown-item" href="be_pages_generic_invoice.html">
                                    <i className="far fa-fw fa-file-alt me-1"></i> Invoices
                                </a>
                                <div role="separator" className="dropdown-divider"></div>

                                <a className="dropdown-item" href="#" data-toggle="layout" data-action="side_overlay_toggle">
                                    <i className="far fa-fw fa-building me-1"></i> Settings
                                </a>

                                <div role="separator" className="dropdown-divider"></div>
                                <a className="dropdown-item" href="op_auth_signin.html">
                                    <i className="far fa-fw fa-arrow-alt-circle-left me-1"></i> Sign Out
                                </a>
                            </div>
                        </div>
                    </div>
                    <div className="dropdown d-inline-block">
                        <button type="button" className="btn btn-alt-secondary" id="page-header-notifications-dropdown" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            <i className="fa fa-fw fa-bell"></i>
                        </button>
                        <div className="dropdown-menu dropdown-menu-lg dropdown-menu-end p-0" aria-labelledby="page-header-notifications-dropdown">
                            <div className="bg-primary-dark rounded-top fw-semibold text-white text-center p-3">
                                Notifications
                            </div>
                            <ul className="nav-items my-2">
                                <li>
                                    <a className="d-flex text-dark py-2" href="#">
                                        <div className="flex-shrink-0 mx-3">
                                            <i className="fa fa-fw fa-check-circle text-success"></i>
                                        </div>
                                        <div className="flex-grow-1 fs-sm pe-2">
                                            <div className="fw-semibold">App was updated to v5.6!</div>
                                            <div className="text-muted">3 min ago</div>
                                        </div>
                                    </a>
                                </li>
                                <li>
                                    <a className="d-flex text-dark py-2" href="#">
                                        <div className="flex-shrink-0 mx-3">
                                            <i className="fa fa-fw fa-user-plus text-info"></i>
                                        </div>
                                        <div className="flex-grow-1 fs-sm pe-2">
                                            <div className="fw-semibold">New Subscriber was added! You now have 2580!</div>
                                            <div className="text-muted">10 min ago</div>
                                        </div>
                                    </a>
                                </li>
                                <li>
                                    <a className="d-flex text-dark py-2" href="#">
                                        <div className="flex-shrink-0 mx-3">
                                            <i className="fa fa-fw fa-times-circle text-danger"></i>
                                        </div>
                                        <div className="flex-grow-1 fs-sm pe-2">
                                            <div className="fw-semibold">Server backup failed to complete!</div>
                                            <div className="text-muted">30 min ago</div>
                                        </div>
                                    </a>
                                </li>
                                <li>
                                    <a className="d-flex text-dark py-2" href="#">
                                        <div className="flex-shrink-0 mx-3">
                                            <i className="fa fa-fw fa-exclamation-circle text-warning"></i>
                                        </div>
                                        <div className="flex-grow-1 fs-sm pe-2">
                                            <div className="fw-semibold">You are running out of space. Please consider upgrading your plan.</div>
                                            <div className="text-muted">1 hour ago</div>
                                        </div>
                                    </a>
                                </li>
                                <li>
                                    <a className="d-flex text-dark py-2" href="#">
                                        <div className="flex-shrink-0 mx-3">
                                            <i className="fa fa-fw fa-plus-circle text-primary"></i>
                                        </div>
                                        <div className="flex-grow-1 fs-sm pe-2">
                                            <div className="fw-semibold">New Sale! + $30</div>
                                            <div className="text-muted">2 hours ago</div>
                                        </div>
                                    </a>
                                </li>
                            </ul>
                            <div className="p-2 border-top">
                                <a className="btn btn-alt-primary w-100 text-center" href="#">
                                    <i className="fa fa-fw fa-eye opacity-50 me-1"></i> View All
                                </a>
                            </div>
                        </div>
                    </div>
                    <button type="button" className="btn btn-alt-secondary" data-toggle="layout" data-action="side_overlay_toggle">
                        <i className="far fa-fw fa-list-alt"></i>
                    </button>
                </div>
            </div>
            <div id="page-header-search" className="overlay-header bg-header-dark">
                <div className="bg-white-10">
                    <div className="content-header">
                        <form className="w-100" action="be_pages_generic_search.html" method="POST">
                            <div className="input-group">
                                <button type="button" className="btn btn-alt-primary" data-toggle="layout" data-action="header_search_off">
                                    <i className="fa fa-fw fa-times-circle"></i>
                                </button>
                                <input type="text" className="form-control border-0" placeholder="Search or hit ESC.." id="page-header-search-input" name="page-header-search-input" />
                            </div>
                        </form>
                    </div>
                </div>
            </div>
            <div id="page-header-loader" className="overlay-header bg-header-dark">
                <div className="bg-white-10">
                    <div className="content-header">
                        <div className="w-100 text-center">
                            <i className="fa fa-fw fa-sun fa-spin text-white"></i>
                        </div>
                    </div>
                </div>
            </div>
        </header>
    );
}

export default observer(PageHeader);