function SideOverlay() {
    return (
        <aside id="side-overlay">
            <div className="bg-image" style={{ backgroundImage: 'url("assets/media/various/bg_side_overlay_header.jpg")' }}>
                <div className="bg-primary-op">
                    <div className="content-header">
                        <a className="img-link me-1" href="#">
                            <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar10.jpg" alt="" />
                        </a>
                        <div className="ms-2">
                            <a className="text-white fw-semibold" href="be_pages_generic_profile.html">George Taylor</a>
                            <div className="text-white-75 fs-sm">Full Stack Developer</div>
                        </div>
                        <a className="ms-auto text-white" href="#" data-toggle="layout" data-action="side_overlay_close">
                            <i className="fa fa-times-circle"></i>
                        </a>
                    </div>
                </div>
            </div>
            <div className="content-side">
                <div className="block block-transparent pull-x pull-t mb-0">
                    <ul className="nav nav-tabs nav-tabs-block nav-justified" role="tablist">
                        <li className="nav-item" role="presentation">
                            <button className="nav-link active" id="so-settings-tab" data-bs-toggle="tab" data-bs-target="#so-settings" role="tab" aria-controls="so-settings" aria-selected="true">
                                <i className="fa fa-fw fa-cog"></i>
                            </button>
                        </li>
                        <li className="nav-item" role="presentation">
                            <button className="nav-link" id="so-people-tab" data-bs-toggle="tab" data-bs-target="#so-people" role="tab" aria-controls="so-people" aria-selected="false">
                                <i className="far fa-fw fa-user-circle"></i>
                            </button>
                        </li>
                        <li className="nav-item" role="presentation">
                            <button className="nav-link" id="so-profile-tab" data-bs-toggle="tab" data-bs-target="#so-profile" role="tab" aria-controls="so-profile" aria-selected="false">
                                <i className="far fa-fw fa-edit"></i>
                            </button>
                        </li>
                    </ul>
                    <div className="block-content tab-content overflow-hidden">
                        <div className="tab-pane pull-x fade fade-up show active" id="so-settings" role="tabpanel" aria-labelledby="so-settings-tab">
                            <div className="block mb-0">
                                <div className="block-content block-content-sm block-content-full bg-body">
                                    <span className="text-uppercase fs-sm fw-bold">Color Themes</span>
                                </div>
                                <div className="block-content block-content-full">
                                    <div className="row g-sm text-center">
                                        <div className="col-4 mb-1">
                                            <a className="d-block py-3 text-white fs-xs fw-semibold bg-default" data-toggle="theme" data-theme="default" href="#">
                                                Default
                                            </a>
                                        </div>
                                        <div className="col-4 mb-1">
                                            <a className="d-block py-3 text-white fs-sm fw-semibold bg-xwork" data-toggle="theme" data-theme="assets/css/themes/xwork.min.css" href="#">
                                                xWork
                                            </a>
                                        </div>
                                        <div className="col-4 mb-1">
                                            <a className="d-block py-3 text-white fs-sm fw-semibold bg-xmodern" data-toggle="theme" data-theme="assets/css/themes/xmodern.min.css" href="#">
                                                xModern
                                            </a>
                                        </div>
                                        <div className="col-4 mb-1">
                                            <a className="d-block py-3 text-white fs-sm fw-semibold bg-xeco" data-toggle="theme" data-theme="assets/css/themes/xeco.min.css" href="#">
                                                xEco
                                            </a>
                                        </div>
                                        <div className="col-4 mb-1">
                                            <a className="d-block py-3 text-white fs-sm fw-semibold bg-xsmooth" data-toggle="theme" data-theme="assets/css/themes/xsmooth.min.css" href="#">
                                                xSmooth
                                            </a>
                                        </div>
                                        <div className="col-4 mb-1">
                                            <a className="d-block py-3 text-white fs-sm fw-semibold bg-xinspire" data-toggle="theme" data-theme="assets/css/themes/xinspire.min.css" href="#">
                                                xInspire
                                            </a>
                                        </div>
                                        <div className="col-4 mb-1">
                                            <a className="d-block py-3 text-white fs-sm fw-semibold bg-xdream" data-toggle="theme" data-theme="assets/css/themes/xdream.min.css" href="#">
                                                xDream
                                            </a>
                                        </div>
                                        <div className="col-4 mb-1">
                                            <a className="d-block py-3 text-white fs-sm fw-semibold bg-xpro" data-toggle="theme" data-theme="assets/css/themes/xpro.min.css" href="#">
                                                xPro
                                            </a>
                                        </div>
                                        <div className="col-4 mb-1">
                                            <a className="d-block py-3 text-white fs-sm fw-semibold bg-xplay" data-toggle="theme" data-theme="assets/css/themes/xplay.min.css" href="#">
                                                xPlay
                                            </a>
                                        </div>
                                        <div className="col-12">
                                            <a className="d-block py-3 bg-body-dark fw-semibold text-dark" href="be_ui_color_themes.html">All Color Themes</a>
                                        </div>
                                    </div>
                                </div>
                                <div className="block-content block-content-sm block-content-full bg-body d-dark-none">
                                    <span className="text-uppercase fs-sm fw-bold">Sidebar</span>
                                </div>
                                <div className="block-content block-content-full d-dark-none">
                                    <div className="row g-sm text-center">
                                        <div className="col-6 mb-1">
                                            <a className="d-block py-3 bg-body-dark fw-semibold text-dark" data-toggle="layout" data-action="sidebar_style_dark" href="#">Dark</a>
                                        </div>
                                        <div className="col-6 mb-1">
                                            <a className="d-block py-3 bg-body-dark fw-semibold text-dark" data-toggle="layout" data-action="sidebar_style_light" href="#">Light</a>
                                        </div>
                                    </div>
                                </div>
                                <div className="block-content block-content-sm block-content-full bg-body">
                                    <span className="text-uppercase fs-sm fw-bold">Header</span>
                                </div>
                                <div className="block-content block-content-full">
                                    <div className="row g-sm text-center mb-2">
                                        <div className="col-6 mb-1 d-dark-none">
                                            <a className="d-block py-3 bg-body-dark fw-semibold text-dark" data-toggle="layout" data-action="header_style_dark" href="#">Dark</a>
                                        </div>
                                        <div className="col-6 mb-1 d-dark-none">
                                            <a className="d-block py-3 bg-body-dark fw-semibold text-dark" data-toggle="layout" data-action="header_style_light" href="#">Light</a>
                                        </div>
                                        <div className="col-6 mb-1">
                                            <a className="d-block py-3 bg-body-dark fw-semibold text-dark" data-toggle="layout" data-action="header_mode_fixed" href="#">Fixed</a>
                                        </div>
                                        <div className="col-6 mb-1">
                                            <a className="d-block py-3 bg-body-dark fw-semibold text-dark" data-toggle="layout" data-action="header_mode_static" href="#">Static</a>
                                        </div>
                                    </div>
                                </div>
                                <div className="block-content block-content-sm block-content-full bg-body">
                                    <span className="text-uppercase fs-sm fw-bold">Content</span>
                                </div>
                                <div className="block-content block-content-full">
                                    <div className="row g-sm text-center">
                                        <div className="col-6 mb-1">
                                            <a className="d-block py-3 bg-body-dark fw-semibold text-dark" data-toggle="layout" data-action="content_layout_boxed" href="#">Boxed</a>
                                        </div>
                                        <div className="col-6 mb-1">
                                            <a className="d-block py-3 bg-body-dark fw-semibold text-dark" data-toggle="layout" data-action="content_layout_narrow" href="#">Narrow</a>
                                        </div>
                                        <div className="col-12 mb-1">
                                            <a className="d-block py-3 bg-body-dark fw-semibold text-dark" data-toggle="layout" data-action="content_layout_full_width" href="#">Full Width</a>
                                        </div>
                                    </div>
                                </div>
                                <div className="block-content block-content-full border-top">
                                    <a className="btn w-100 btn-alt-primary" href="be_layout_api.html">
                                        <i className="fa fa-fw fa-flask me-1"></i> Layout API
                                    </a>
                                </div>
                            </div>
                        </div>
                        <div className="tab-pane pull-x fade fade-up" id="so-people" role="tabpanel" aria-labelledby="so-people-tab">
                            <div className="block mb-0">
                                <div className="block-content block-content-sm block-content-full bg-body">
                                    <span className="text-uppercase fs-sm fw-bold">Online</span>
                                </div>
                                <div className="block-content">
                                    <ul className="nav-items">
                                        <li>
                                            <a className="d-flex py-2" href="be_pages_generic_profile.html">
                                                <div className="flex-shrink-0 mx-3 overlay-container">
                                                    <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar1.jpg" alt="" />
                                                    <span className="overlay-item item item-tiny item-circle border border-2 border-white bg-success"></span>
                                                </div>
                                                <div className="flex-grow-1">
                                                    <div className="fw-semibold">Betty Kelley</div>
                                                    <div className="fs-sm text-muted">Photographer</div>
                                                </div>
                                            </a>
                                        </li>
                                        <li>
                                            <a className="d-flex py-2" href="be_pages_generic_profile.html">
                                                <div className="flex-shrink-0 mx-3 overlay-container">
                                                    <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar16.jpg" alt="" />
                                                    <span className="overlay-item item item-tiny item-circle border border-2 border-white bg-success"></span>
                                                </div>
                                                <div className="flex-grow-1">
                                                    <div className="fw-semibold">Jose Parker</div>
                                                    <div className="fw-normal fs-sm text-muted">Web Designer</div>
                                                </div>
                                            </a>
                                        </li>
                                        <li>
                                            <a className="d-flex py-2" href="be_pages_generic_profile.html">
                                                <div className="flex-shrink-0 mx-3 overlay-container">
                                                    <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar5.jpg" alt="" />
                                                    <span className="overlay-item item item-tiny item-circle border border-2 border-white bg-success"></span>
                                                </div>
                                                <div className="flex-grow-1">
                                                    <div className="fw-semibold">Amber Harvey</div>
                                                    <div className="fw-normal fs-sm text-muted">Web Developer</div>
                                                </div>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                                <div className="block-content block-content-sm block-content-full bg-body">
                                    <span className="text-uppercase fs-sm fw-bold">Busy</span>
                                </div>
                                <div className="block-content">
                                    <ul className="nav-items">
                                        <li>
                                            <a className="d-flex py-2" href="be_pages_generic_profile.html">
                                                <div className="flex-shrink-0 mx-3 overlay-container">
                                                    <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar5.jpg" alt="" />
                                                    <span className="overlay-item item item-tiny item-circle border border-2 border-white bg-danger"></span>
                                                </div>
                                                <div className="flex-grow-1">
                                                    <div className="fw-semibold">Laura Carr</div>
                                                    <div className="fw-normal fs-sm text-muted">UI Designer</div>
                                                </div>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                                <div className="block-content block-content-sm block-content-full bg-body">
                                    <span className="text-uppercase fs-sm fw-bold">Away</span>
                                </div>
                                <div className="block-content">
                                    <ul className="nav-items">
                                        <li>
                                            <a className="d-flex py-2" href="be_pages_generic_profile.html">
                                                <div className="flex-shrink-0 mx-3 overlay-container">
                                                    <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar11.jpg" alt="" />
                                                    <span className="overlay-item item item-tiny item-circle border border-2 border-white bg-warning"></span>
                                                </div>
                                                <div className="flex-grow-1">
                                                    <div className="fw-semibold">Brian Cruz</div>
                                                    <div className="fw-normal fs-sm text-muted">Copywriter</div>
                                                </div>
                                            </a>
                                        </li>
                                        <li>
                                            <a className="d-flex py-2" href="be_pages_generic_profile.html">
                                                <div className="flex-shrink-0 mx-3 overlay-container">
                                                    <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar2.jpg" alt="" />
                                                    <span className="overlay-item item item-tiny item-circle border border-2 border-white bg-warning"></span>
                                                </div>
                                                <div className="flex-grow-1">
                                                    <div className="fw-semibold">Megan Fuller</div>
                                                    <div className="fw-normal fs-sm text-muted">Writer</div>
                                                </div>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                                <div className="block-content block-content-sm block-content-full bg-body">
                                    <span className="text-uppercase fs-sm fw-bold">Offline</span>
                                </div>
                                <div className="block-content">
                                    <ul className="nav-items">
                                        <li>
                                            <a className="d-flex py-2" href="be_pages_generic_profile.html">
                                                <div className="flex-shrink-0 mx-3 overlay-container">
                                                    <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar14.jpg" alt="" />
                                                    <span className="overlay-item item item-tiny item-circle border border-2 border-white bg-muted"></span>
                                                </div>
                                                <div className="flex-grow-1">
                                                    <div className="fw-semibold">Brian Cruz</div>
                                                    <div className="fw-normal fs-sm text-muted">Teacher</div>
                                                </div>
                                            </a>
                                        </li>
                                        <li>
                                            <a className="d-flex py-2" href="be_pages_generic_profile.html">
                                                <div className="flex-shrink-0 mx-3 overlay-container">
                                                    <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar5.jpg" alt="" />
                                                    <span className="overlay-item item item-tiny item-circle border border-2 border-white bg-muted"></span>
                                                </div>
                                                <div className="flex-grow-1">
                                                    <div className="fw-semibold">Megan Fuller</div>
                                                    <div className="fw-normal fs-sm text-muted">Photographer</div>
                                                </div>
                                            </a>
                                        </li>
                                        <li>
                                            <a className="d-flex py-2" href="be_pages_generic_profile.html">
                                                <div className="flex-shrink-0 mx-3 overlay-container">
                                                    <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar1.jpg" alt="" />
                                                    <span className="overlay-item item item-tiny item-circle border border-2 border-white bg-muted"></span>
                                                </div>
                                                <div className="flex-grow-1">
                                                    <div className="fw-semibold">Helen Jacobs</div>
                                                    <div className="fw-normal fs-sm text-muted">Front-end Developer</div>
                                                </div>
                                            </a>
                                        </li>
                                        <li>
                                            <a className="d-flex py-2" href="be_pages_generic_profile.html">
                                                <div className="flex-shrink-0 mx-3 overlay-container">
                                                    <img className="img-avatar img-avatar48" src="assets/media/avatars/avatar9.jpg" alt="" />
                                                    <span className="overlay-item item item-tiny item-circle border border-2 border-white bg-muted"></span>
                                                </div>
                                                <div className="flex-grow-1">
                                                    <div className="fw-semibold">Jeffrey Shaw</div>
                                                    <div className="fw-normal fs-sm text-muted">UX Specialist</div>
                                                </div>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                                <div className="block-content block-content-full border-top">
                                    <a className="btn w-100 btn-alt-primary" href="#">
                                        <i className="fa fa-fw fa-plus me-1 opacity-50"></i> Add People
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </aside>
    );
}

export default SideOverlay;