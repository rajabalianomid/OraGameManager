import { useStore } from "../Store";

function Slidebar() {

    const { mainStore } = useStore();

    return (
        <nav id="sidebar" aria-label="Main Navigation">
            <div className="bg-header-dark">
                <div className="content-header bg-white-5">
                    <a className="fw-semibold text-white tracking-wide" href="index.html">
                        <span className="smini-visible">
                            D<span className="opacity-75">x</span>
                        </span>
                        <span className="smini-hidden">
                            Dash<span className="opacity-75">mix</span>
                        </span>
                    </a>
                    <div className="d-flex align-items-center gap-1">
                        <div className="dropdown">
                            <button type="button" className="btn btn-sm btn-alt-secondary" id="sidebar-dark-mode-dropdown" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <i className="far fa-fw fa-moon" data-dark-mode-icon></i>
                            </button>
                            <div className="dropdown-menu dropdown-menu-end smini-hide border-0" aria-labelledby="sidebar-dark-mode-dropdown">
                                <button type="button" className="dropdown-item d-flex align-items-center gap-2" data-toggle="layout" data-action="dark_mode_off" data-dark-mode="off">
                                    <i className="far fa-sun fa-fw opacity-50"></i>
                                    <span className="fs-sm fw-medium">Light</span>
                                </button>
                                <button type="button" className="dropdown-item d-flex align-items-center gap-2" data-toggle="layout" data-action="dark_mode_on" data-dark-mode="on">
                                    <i className="far fa-moon fa-fw opacity-50"></i>
                                    <span className="fs-sm fw-medium">Dark</span>
                                </button>
                                <button type="button" className="dropdown-item d-flex align-items-center gap-2" data-toggle="layout" data-action="dark_mode_system" data-dark-mode="system">
                                    <i className="fa fa-desktop fa-fw opacity-50"></i>
                                    <span className="fs-sm fw-medium">System</span>
                                </button>
                            </div>
                        </div>
                        <div className="dropdown">
                            <button type="button" className="btn btn-sm btn-alt-secondary" id="sidebar-themes-dropdown" data-bs-auto-close="outside" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <i className="fa fa-fw fa-paint-brush"></i>
                            </button>
                            <div className="dropdown-menu dropdown-menu-end fs-sm border-0" aria-labelledby="sidebar-themes-dropdown">
                                <div className="row g-sm text-center">
                                    <div className="col-4 mb-1">
                                        <a className="d-block py-3 text-white fs-xs fw-semibold bg-default rounded-1" data-toggle="theme" data-theme="default" href="#">
                                            Default
                                        </a>
                                    </div>
                                    <div className="col-4 mb-1">
                                        <a className="d-block py-3 text-white fs-xs fw-semibold bg-xwork rounded-1" data-toggle="theme" data-theme="assets/css/themes/xwork.min.css" href="#">
                                            xWork
                                        </a>
                                    </div>
                                    <div className="col-4 mb-1">
                                        <a className="d-block py-3 text-white fs-xs fw-semibold bg-xmodern rounded-1" data-toggle="theme" data-theme="assets/css/themes/xmodern.min.css" href="#">
                                            xModern
                                        </a>
                                    </div>
                                    <div className="col-4 mb-1">
                                        <a className="d-block py-3 text-white fs-xs fw-semibold bg-xeco rounded-1" data-toggle="theme" data-theme="assets/css/themes/xeco.min.css" href="#">
                                            xEco
                                        </a>
                                    </div>
                                    <div className="col-4 mb-1">
                                        <a className="d-block py-3 text-white fs-xs fw-semibold bg-xsmooth rounded-1" data-toggle="theme" data-theme="assets/css/themes/xsmooth.min.css" href="#">
                                            xSmooth
                                        </a>
                                    </div>
                                    <div className="col-4 mb-1">
                                        <a className="d-block py-3 text-white fs-xs fw-semibold bg-xinspire rounded-1" data-toggle="theme" data-theme="assets/css/themes/xinspire.min.css" href="#">
                                            xInspire
                                        </a>
                                    </div>
                                    <div className="col-4 mb-1">
                                        <a className="d-block py-3 text-white fs-xs fw-semibold bg-xdream rounded-1" data-toggle="theme" data-theme="assets/css/themes/xdream.min.css" href="#">
                                            xDream
                                        </a>
                                    </div>
                                    <div className="col-4 mb-1">
                                        <a className="d-block py-3 text-white fs-xs fw-semibold bg-xpro rounded-1" data-toggle="theme" data-theme="assets/css/themes/xpro.min.css" href="#">
                                            xPro
                                        </a>
                                    </div>
                                    <div className="col-4 mb-1">
                                        <a className="d-block py-3 text-white fs-xs fw-semibold bg-xplay rounded-1" data-toggle="theme" data-theme="assets/css/themes/xplay.min.css" href="#">
                                            xPlay
                                        </a>
                                    </div>
                                    <div className="col-12">
                                        <a className="d-block py-2 bg-body-dark fs-xs fw-semibold text-dark rounded-1" href="be_ui_color_themes.html">All Color Themes</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <button type="button" className="btn btn-sm btn-alt-secondary d-lg-none" data-toggle="layout" data-action="sidebar_close" onClick={mainStore.sidebarToggle}>
                            <i className="fa fa-times-circle"></i>
                        </button>
                    </div>
                </div>
            </div>
            <div className="js-sidebar-scroll">
                <div className="content-side">
                    <ul className="nav-main">
                        <li className="nav-main-item">
                            <a className="nav-main-link active" href="#">
                                <i className="nav-main-link-icon fa fa-location-arrow"></i>
                                <span className="nav-main-link-name">Dashboard</span>
                                <span className="nav-main-link-badge badge rounded-pill bg-primary">8</span>
                            </a>
                        </li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-clone"></i>
                                <span className="nav-main-link-name">Page Kits</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Generic</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_blank.html">
                                                <span className="nav-main-link-name">Blank</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_blank_block.html">
                                                <span className="nav-main-link-name">Blank (Block)</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_search.html">
                                                <span className="nav-main-link-name">Search</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_pricing_plans.html">
                                                <span className="nav-main-link-name">Pricing Plans</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_profile.html">
                                                <span className="nav-main-link-name">Profile</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_profile_edit.html">
                                                <span className="nav-main-link-name">Profile Edit</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_profile_v2.html">
                                                <span className="nav-main-link-name">Profile v2</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_profile_v2_edit.html">
                                                <span className="nav-main-link-name">Profile v2 Edit</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_inbox.html">
                                                <span className="nav-main-link-name">Inbox</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_invoice.html">
                                                <span className="nav-main-link-name">Invoice</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_faq.html">
                                                <span className="nav-main-link-name">FAQ</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_generic_upgrade_plan.html">
                                                <span className="nav-main-link-name">Upgrade Plan</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_sidebar_mini_nav.html">
                                                <span className="nav-main-link-name">Sidebar with Mini Nav</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">e-Commerce</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_ecom_dashboard.html">
                                                <span className="nav-main-link-name">Dashboard</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_ecom_orders.html">
                                                <span className="nav-main-link-name">Orders</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_ecom_order.html">
                                                <span className="nav-main-link-name">Order</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_ecom_products.html">
                                                <span className="nav-main-link-name">Products</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_ecom_product_edit.html">
                                                <span className="nav-main-link-name">Product Edit</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_ecom_customer.html">
                                                <span className="nav-main-link-name">Customer</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Projects</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_projects_dashboard.html">
                                                <span className="nav-main-link-name">Dashboard</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_projects_tasks.html">
                                                <span className="nav-main-link-name">Project Tasks</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_projects_create.html">
                                                <span className="nav-main-link-name">Create</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_projects_edit.html">
                                                <span className="nav-main-link-name">Edit</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Jobs</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_jobs_dashboard.html">
                                                <span className="nav-main-link-name">Dashboard</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_jobs_listing.html">
                                                <span className="nav-main-link-name">Listing</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_jobs_apply.html">
                                                <span className="nav-main-link-name">Apply</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_jobs_post.html">
                                                <span className="nav-main-link-name">Post</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Education</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_education_dashboard.html">
                                                <span className="nav-main-link-name">Dashboard</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_education_course.html">
                                                <span className="nav-main-link-name">Course</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_education_authors.html">
                                                <span className="nav-main-link-name">Authors</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Forum</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_forum_categories.html">
                                                <span className="nav-main-link-name">Categories</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_forum_topics.html">
                                                <span className="nav-main-link-name">Topics</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_forum_discussion.html">
                                                <span className="nav-main-link-name">Discussion</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Blog</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_blog_classNameic.html">
                                                <span className="nav-main-link-name">classNameic</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_blog_list.html">
                                                <span className="nav-main-link-name">List</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_blog_grid.html">
                                                <span className="nav-main-link-name">Grid</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_blog_story.html">
                                                <span className="nav-main-link-name">Story</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_blog_story_cover.html">
                                                <span className="nav-main-link-name">Story Cover</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_blog_post_manage.html">
                                                <span className="nav-main-link-name">Manage Posts</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_blog_post_add.html">
                                                <span className="nav-main-link-name">Add Post</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_pages_blog_post_edit.html">
                                                <span className="nav-main-link-name">Edit Post</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Boxed Backend</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="bd_dashboard.html">
                                                <span className="nav-main-link-name">Dashboard</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="bd_search.html">
                                                <span className="nav-main-link-name">Search</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="bd_simple_1.html">
                                                <span className="nav-main-link-name">Simple 1</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="bd_simple_2.html">
                                                <span className="nav-main-link-name">Simple 2</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="bd_image_1.html">
                                                <span className="nav-main-link-name">Image 1</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="bd_image_2.html">
                                                <span className="nav-main-link-name">Image 2</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="bd_video_1.html">
                                                <span className="nav-main-link-name">Video 1</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="bd_video_2.html">
                                                <span className="nav-main-link-name">Video 2</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Special</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="op_checkout.html">
                                                <span className="nav-main-link-name">Checkout</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="op_maintenance.html">
                                                <span className="nav-main-link-name">Maintenance</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="op_status.html">
                                                <span className="nav-main-link-name">Status</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="op_installation.html">
                                                <span className="nav-main-link-name">Installation</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="op_coming_soon.html">
                                                <span className="nav-main-link-name">Coming Soon</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="op_coming_soon_2.html">
                                                <span className="nav-main-link-name">Coming Soon 2</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-heading">Base</li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-border-all"></i>
                                <span className="nav-main-link-name">Blocks</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_blocks_styles.html">
                                        <span className="nav-main-link-name">Styles</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_blocks_options.html">
                                        <span className="nav-main-link-name">Options</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_blocks_forms.html">
                                        <span className="nav-main-link-name">Forms</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_blocks_themed.html">
                                        <span className="nav-main-link-name">Themed</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_blocks_api.html">
                                        <span className="nav-main-link-name">API</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-boxes"></i>
                                <span className="nav-main-link-name">Widgets</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_widgets_tiles.html">
                                        <span className="nav-main-link-name">Tiles</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_widgets_stats.html">
                                        <span className="nav-main-link-name">Statistics</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_widgets_media.html">
                                        <span className="nav-main-link-name">Media</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_widgets_users.html">
                                        <span className="nav-main-link-name">Users</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_widgets_blog.html">
                                        <span className="nav-main-link-name">Blog</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_widgets_various.html">
                                        <span className="nav-main-link-name">Various</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-vector-square"></i>
                                <span className="nav-main-link-name">Layout</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Page</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_page_default.html">
                                                <span className="nav-main-link-name">Default</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_page_flipped.html">
                                                <span className="nav-main-link-name">Flipped</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_page_native_scrolling.html">
                                                <span className="nav-main-link-name">Native Scrolling</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Main Content</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_content_main_full_width.html">
                                                <span className="nav-main-link-name">Full Width</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_content_main_narrow.html">
                                                <span className="nav-main-link-name">Narrow</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_content_main_boxed.html">
                                                <span className="nav-main-link-name">Boxed</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Side Content</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_content_side_left.html">
                                                <span className="nav-main-link-name">Left</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_content_side_right.html">
                                                <span className="nav-main-link-name">Right</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Hero</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                                <span className="nav-main-link-name">Color</span>
                                            </a>
                                            <ul className="nav-main-submenu">
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_hero_color_dark.html">
                                                        <span className="nav-main-link-name">Dark</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_hero_color_light.html">
                                                        <span className="nav-main-link-name">Light</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                                <span className="nav-main-link-name">Image</span>
                                            </a>
                                            <ul className="nav-main-submenu">
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_hero_image_dark.html">
                                                        <span className="nav-main-link-name">Dark</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_hero_image_light.html">
                                                        <span className="nav-main-link-name">Light</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                                <span className="nav-main-link-name">Video</span>
                                            </a>
                                            <ul className="nav-main-submenu">
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_hero_video_dark.html">
                                                        <span className="nav-main-link-name">Dark</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_hero_video_light.html">
                                                        <span className="nav-main-link-name">Light</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Header</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                                <span className="nav-main-link-name">Fixed</span>
                                            </a>
                                            <ul className="nav-main-submenu">
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_header_fixed_light.html">
                                                        <span className="nav-main-link-name">Light</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_header_fixed_light_glass.html">
                                                        <span className="nav-main-link-name">Light Glass</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_header_fixed_dark.html">
                                                        <span className="nav-main-link-name">Dark</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_header_fixed_dark_glass.html">
                                                        <span className="nav-main-link-name">Dark Glass</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                                <span className="nav-main-link-name">Static</span>
                                            </a>
                                            <ul className="nav-main-submenu">
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_header_static_light.html">
                                                        <span className="nav-main-link-name">Light</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_header_static_light_glass.html">
                                                        <span className="nav-main-link-name">Light Glass</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_header_static_dark.html">
                                                        <span className="nav-main-link-name">Dark</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="be_layout_header_static_dark_glass.html">
                                                        <span className="nav-main-link-name">Dark Glass</span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Footer</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_footer_static.html">
                                                <span className="nav-main-link-name">Static</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_footer_fixed.html">
                                                <span className="nav-main-link-name">Fixed</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Sidebar</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_sidebar_mini.html">
                                                <span className="nav-main-link-name">Mini</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_sidebar_light.html">
                                                <span className="nav-main-link-name">Light</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_sidebar_dark.html">
                                                <span className="nav-main-link-name">Dark</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_sidebar_hidden.html">
                                                <span className="nav-main-link-name">Hidden</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Side Overlay</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_side_overlay_visible.html">
                                                <span className="nav-main-link-name">Visible</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_side_overlay_mode_hover.html">
                                                <span className="nav-main-link-name">Hover Mode</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_layout_side_overlay_no_page_overlay.html">
                                                <span className="nav-main-link-name">No Page Overlay</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_layout_api.html">
                                        <span className="nav-main-link-name">API</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-heading">Interface</li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-flask"></i>
                                <span className="nav-main-link-name">Elements</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_icons.html">
                                        <span className="nav-main-link-name">Icon Packs</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_grid.html">
                                        <span className="nav-main-link-name">Grid</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_typography.html">
                                        <span className="nav-main-link-name">Typography</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_navigation.html">
                                        <span className="nav-main-link-name">Navigation</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_navigation_horizontal.html">
                                        <span className="nav-main-link-name">Horizontal Nav</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_mega_menu.html">
                                        <span className="nav-main-link-name">Mega Menu</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_tabs.html">
                                        <span className="nav-main-link-name">Tabs</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_buttons.html">
                                        <span className="nav-main-link-name">Buttons</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_buttons_groups.html">
                                        <span className="nav-main-link-name">Button Groups</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_dropdowns.html">
                                        <span className="nav-main-link-name">Dropdowns</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_progress.html">
                                        <span className="nav-main-link-name">Progress</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_alerts.html">
                                        <span className="nav-main-link-name">Alerts</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_tooltips.html">
                                        <span className="nav-main-link-name">Tooltips</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_popovers.html">
                                        <span className="nav-main-link-name">Popovers</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_accordion.html">
                                        <span className="nav-main-link-name">Accordion</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_modals.html">
                                        <span className="nav-main-link-name">Modals</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_images.html">
                                        <span className="nav-main-link-name">Images Overlay</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_timeline.html">
                                        <span className="nav-main-link-name">Timeline</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_ribbons.html">
                                        <span className="nav-main-link-name">Ribbons</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_steps.html">
                                        <span className="nav-main-link-name">Steps</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_animations.html">
                                        <span className="nav-main-link-name">Animations</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_backgrounds.html">
                                        <span className="nav-main-link-name">Backgrounds</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_ui_color_themes.html">
                                        <span className="nav-main-link-name">Color Themes</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-grip-horizontal"></i>
                                <span className="nav-main-link-name">Tables</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_tables_styles.html">
                                        <span className="nav-main-link-name">Styles</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_tables_responsive.html">
                                        <span className="nav-main-link-name">Responsive</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_tables_helpers.html">
                                        <span className="nav-main-link-name">Helpers</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_tables_datatables.html">
                                        <span className="nav-main-link-name">DataTables</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-sticky-note"></i>
                                <span className="nav-main-link-name">Forms</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_forms_elements.html">
                                        <span className="nav-main-link-name">Elements</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_forms_layouts.html">
                                        <span className="nav-main-link-name">Layouts</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_forms_input_groups.html">
                                        <span className="nav-main-link-name">Input Groups</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_forms_plugins.html">
                                        <span className="nav-main-link-name">Plugins</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_forms_editors.html">
                                        <span className="nav-main-link-name">Editors</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">CKEditor 5</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_forms_ckeditor5_classNameic.html">
                                                <span className="nav-main-link-name">classNameic</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="be_forms_ckeditor5_inline.html">
                                                <span className="nav-main-link-name">Inline</span>
                                            </a>
                                        </li>
                                    </ul>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_forms_validation.html">
                                        <span className="nav-main-link-name">Validation</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-heading">Extend</li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-wrench"></i>
                                <span className="nav-main-link-name">Components</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_chat.html">
                                        <span className="nav-main-link-name">Chat</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_onboarding.html">
                                        <span className="nav-main-link-name">Onboarding</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_nestable.html">
                                        <span className="nav-main-link-name">Nestable</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_dialogs.html">
                                        <span className="nav-main-link-name">Dialogs</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_notifications.html">
                                        <span className="nav-main-link-name">Notifications</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_loaders.html">
                                        <span className="nav-main-link-name">Loaders</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_charts.html">
                                        <span className="nav-main-link-name">Charts</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_gallery.html">
                                        <span className="nav-main-link-name">Gallery</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_sliders.html">
                                        <span className="nav-main-link-name">Sliders</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_carousel.html">
                                        <span className="nav-main-link-name">Carousel</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_offcanvas.html">
                                        <span className="nav-main-link-name">Offcanvas</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_rating.html">
                                        <span className="nav-main-link-name">Rating</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_appear.html">
                                        <span className="nav-main-link-name">Appear</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_calendar.html">
                                        <span className="nav-main-link-name">Calendar</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_image_cropper.html">
                                        <span className="nav-main-link-name">Image Cropper</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_maps_vector.html">
                                        <span className="nav-main-link-name">Vector Maps</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_comp_syntax_highlighting.html">
                                        <span className="nav-main-link-name">Syntax Highlighting</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-cog"></i>
                                <span className="nav-main-link-name">Main Menu</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="#">
                                        <i className="nav-main-link-icon fa fa-inbox"></i>
                                        <span className="nav-main-link-name">1.1 Item</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="#">
                                        <i className="nav-main-link-icon fa fa-fire"></i>
                                        <span className="nav-main-link-name">1.2 Item</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="#">
                                        <i className="nav-main-link-icon fa fa-share-alt"></i>
                                        <span className="nav-main-link-name">1.3 Item</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                        <span className="nav-main-link-name">Sub Level 2</span>
                                        <span className="nav-main-link-badge badge rounded-pill bg-primary">3</span>
                                    </a>
                                    <ul className="nav-main-submenu">
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="#">
                                                <i className="nav-main-link-icon fa fa-tag"></i>
                                                <span className="nav-main-link-name">2.1 Item</span>
                                                <span className="nav-main-link-badge badge rounded-pill bg-info">2</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="#">
                                                <i className="nav-main-link-icon fa fa-chart-pie"></i>
                                                <span className="nav-main-link-name">2.2 Item</span>
                                                <span className="nav-main-link-badge badge rounded-pill bg-danger">2</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link" href="#">
                                                <i className="nav-main-link-icon fa fa-sticky-note"></i>
                                                <span className="nav-main-link-name">2.3 Item</span>
                                                <span className="nav-main-link-badge badge rounded-pill bg-warning">3</span>
                                            </a>
                                        </li>
                                        <li className="nav-main-item">
                                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                                <span className="nav-main-link-name">Sub Level 3</span>
                                            </a>
                                            <ul className="nav-main-submenu">
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="#">
                                                        <i className="nav-main-link-icon fa fa-map"></i>
                                                        <span className="nav-main-link-name">3.1 Item</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="#">
                                                        <i className="nav-main-link-icon fa fa-coffee"></i>
                                                        <span className="nav-main-link-name">3.2 Item</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link" href="#">
                                                        <i className="nav-main-link-icon fa fa-user-astronaut"></i>
                                                        <span className="nav-main-link-name">3.3 Item</span>
                                                    </a>
                                                </li>
                                                <li className="nav-main-item">
                                                    <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                                        <span className="nav-main-link-name">Sub Level 4</span>
                                                    </a>
                                                    <ul className="nav-main-submenu">
                                                        <li className="nav-main-item">
                                                            <a className="nav-main-link" href="#">
                                                                <i className="nav-main-link-icon fa fa-heart"></i>
                                                                <span className="nav-main-link-name">4.1 Item</span>
                                                            </a>
                                                        </li>
                                                        <li className="nav-main-item">
                                                            <a className="nav-main-link" href="#">
                                                                <i className="nav-main-link-icon fa fa-search"></i>
                                                                <span className="nav-main-link-name">4.2 Item</span>
                                                            </a>
                                                        </li>
                                                        <li className="nav-main-item">
                                                            <a className="nav-main-link" href="#">
                                                                <i className="nav-main-link-icon fa fa-microphone"></i>
                                                                <span className="nav-main-link-name">4.3 Item</span>
                                                            </a>
                                                        </li>
                                                    </ul>
                                                </li>
                                            </ul>
                                        </li>
                                    </ul>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-heading">Pages</li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-rocket"></i>
                                <span className="nav-main-link-name">Dashboards</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_pages_dashboard_all.html">
                                        <span className="nav-main-link-name">All</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_pages_dashboard_v1.html">
                                        <span className="nav-main-link-name">Corporate v1</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_social.html">
                                        <span className="nav-main-link-name">Social</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_messages.html">
                                        <span className="nav-main-link-name">Messages</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_minimal.html">
                                        <span className="nav-main-link-name">Minimal</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_modern.html">
                                        <span className="nav-main-link-name">Modern</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_classNameic_boxed.html">
                                        <span className="nav-main-link-name">classNameic Boxed</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_banking.html">
                                        <span className="nav-main-link-name">Banking</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_crypto.html">
                                        <span className="nav-main-link-name">Crypto</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_hosting.html">
                                        <span className="nav-main-link-name">Hosting</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_booking.html">
                                        <span className="nav-main-link-name">Booking</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_gaming.html">
                                        <span className="nav-main-link-name">Gaming</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_tasks.html">
                                        <span className="nav-main-link-name">Tasks</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_medical.html">
                                        <span className="nav-main-link-name">Medical</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_travel.html">
                                        <span className="nav-main-link-name">Travel</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_social_compact.html">
                                        <span className="nav-main-link-name">Social Compact</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_chat.html">
                                        <span className="nav-main-link-name">Chat</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_analytics.html">
                                        <span className="nav-main-link-name">Analytics</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_corporate_slim.html">
                                        <span className="nav-main-link-name">Corporate Slim</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_wp_post.html">
                                        <span className="nav-main-link-name">WP Post</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="db_file_hosting.html">
                                        <span className="nav-main-link-name">File Hosting</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-user-friends"></i>
                                <span className="nav-main-link-name">Auth</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_pages_auth_all.html">
                                        <span className="nav-main-link-name">All</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_signin.html">
                                        <span className="nav-main-link-name">Sign In</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_signin_box.html">
                                        <span className="nav-main-link-name">Sign In Box</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_signin_box_alt.html">
                                        <span className="nav-main-link-name">Sign In Box Alt</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_signup.html">
                                        <span className="nav-main-link-name">Sign Up</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_signup_box.html">
                                        <span className="nav-main-link-name">Sign Up Box</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_signup_box_alt.html">
                                        <span className="nav-main-link-name">Sign Up Box Alt</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_lock.html">
                                        <span className="nav-main-link-name">Lock Screen</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_lock_box.html">
                                        <span className="nav-main-link-name">Lock Screen Box</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_lock_box_alt.html">
                                        <span className="nav-main-link-name">Lock Screen Box Alt</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_reminder.html">
                                        <span className="nav-main-link-name">Pass Reminder</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_reminder_box.html">
                                        <span className="nav-main-link-name">Pass Reminder Box</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_reminder_box_alt.html">
                                        <span className="nav-main-link-name">Pass Reminder Box Alt</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_two_factor.html">
                                        <span className="nav-main-link-name">Two Factor</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_two_factor_box.html">
                                        <span className="nav-main-link-name">Two Factor Box</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_auth_two_factor_box_alt.html">
                                        <span className="nav-main-link-name">Two Factor Box Alt</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-ghost"></i>
                                <span className="nav-main-link-name">Error</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="be_pages_error_all.html">
                                        <span className="nav-main-link-name">All</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_error_400.html">
                                        <span className="nav-main-link-name">400</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_error_401.html">
                                        <span className="nav-main-link-name">401</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_error_403.html">
                                        <span className="nav-main-link-name">403</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_error_404.html">
                                        <span className="nav-main-link-name">404</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_error_500.html">
                                        <span className="nav-main-link-name">500</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="op_error_503.html">
                                        <span className="nav-main-link-name">503</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                        <li className="nav-main-item">
                            <a className="nav-main-link nav-main-link-submenu" data-toggle="submenu" aria-haspopup="true" aria-expanded="false" href="#">
                                <i className="nav-main-link-icon fa fa-coffee"></i>
                                <span className="nav-main-link-name">Get Started</span>
                            </a>
                            <ul className="nav-main-submenu">
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="gs_backend.html">
                                        <span className="nav-main-link-name">Backend</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="gs_backend_boxed.html">
                                        <span className="nav-main-link-name">Backend Boxed</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="gs_landing.html">
                                        <span className="nav-main-link-name">Landing</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="gs_rtl_backend.html">
                                        <span className="nav-main-link-name">RTL Backend</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="gs_rtl_backend_boxed.html">
                                        <span className="nav-main-link-name">RTL Backend Boxed</span>
                                    </a>
                                </li>
                                <li className="nav-main-item">
                                    <a className="nav-main-link" href="gs_rtl_landing.html">
                                        <span className="nav-main-link-name">RTL Landing</span>
                                    </a>
                                </li>
                            </ul>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    );

}

export default Slidebar;