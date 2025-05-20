function PageFooter() {
    return (
        <footer id="page-footer" className="bg-body-light">
            <div className="content py-0">
                <div className="row fs-sm">
                    <div className="col-sm-6 order-sm-2 mb-1 mb-sm-0 text-center text-sm-end">
                        Crafted with <i className="fa fa-heart text-danger"></i> by <a className="fw-semibold" href="https://pixelcave.com" target="_blank">OMafia</a>
                    </div>
                    <div className="col-sm-6 order-sm-1 text-center text-sm-start">
                        <a className="fw-semibold" href="https://pixelcave.com/products/dashmix" target="_blank">Omafia 1.0</a> &copy; <span data-toggle="year-copy"></span>
                    </div>
                </div>
            </div>
        </footer>
    );
}
export default PageFooter;