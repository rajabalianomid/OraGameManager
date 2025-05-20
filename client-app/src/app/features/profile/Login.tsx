import { useState } from "react";
import { LoginModel } from "../../models/LoginModel";
import { LoginValidator } from "../../validations/LoginValidator";
import { useStore } from "../../Store";

function Login() {

    const { profileStore } = useStore();

    const [loginModel, setLoginModel] = useState<LoginModel>({
        form: {
            email: '',
            password: '',
        },
        errors: {},
    });
    const validator = new LoginValidator();

    // Handle input changes
    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;

        setLoginModel((prevModel) => ({
            ...prevModel,
            form: {
                ...prevModel.form,
                [name]: value,
            },
        }));
    };

    // Handle form submission
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        // Validate the form
        const validatedModel = validator.validate(loginModel);

        if (Object.keys(validatedModel.errors).length > 0) {
            setLoginModel(validatedModel);
        } else {
            setLoginModel({ ...validatedModel, errors: {} });
            await profileStore.login(loginModel);
        }
    };

    return (
        <div id="page-container">
            <main id="main-container">
                <div className="bg-image" style={{ backgroundImage: 'url("assets/media/photos/photo22@2x.jpg")' }}>
                    <div className="row g-0 bg-primary-op">
                        <div className="hero-static col-md-6 d-flex align-items-center bg-body-extra-light">
                            <div className="p-3 w-100">
                                <div className="mb-3 text-center">
                                    <a className="link-fx fw-bold fs-1" href="index.html">
                                        <span className="text-dark">Dash</span>
                                        <span className="text-primary">mix</span>
                                    </a>
                                    <p className="text-uppercase fw-bold fs-sm text-muted">Sign In</p>
                                </div>
                                <div className="row g-0 justify-content-center">
                                    <div className="col-sm-8 col-xl-6">
                                        <form onSubmit={handleSubmit} noValidate>
                                            <div className="py-3">
                                                <div className="mb-4">
                                                    <input
                                                        type="email"
                                                        className={`form-control form-control-lg form-control-alt ${loginModel.errors.email ? 'is-invalid' : ''}`}
                                                        id="login-email"
                                                        name="email"
                                                        placeholder="Email"
                                                        value={loginModel.form.email}
                                                        onChange={handleInputChange}
                                                    />
                                                    {loginModel.errors.email && (
                                                        <div className="invalid-feedback animated fadeIn">{loginModel.errors.email}</div>
                                                    )}
                                                </div>
                                                <div className="mb-4">
                                                    <input
                                                        type="password"
                                                        className={`form-control form-control-lg form-control-alt ${loginModel.errors.password ? 'is-invalid' : ''}`}
                                                        id="login-password"
                                                        name="password"
                                                        placeholder="Password"
                                                        value={loginModel.form.password}
                                                        onChange={handleInputChange}
                                                    />
                                                    {loginModel.errors.password && (
                                                        <div className="invalid-feedback animated fadeIn">{loginModel.errors.password}</div>
                                                    )}
                                                </div>
                                            </div>
                                            <div className="mb-4">
                                                <button type="submit" className="btn w-100 btn-lg btn-hero btn-primary">
                                                    <i className="fa fa-fw fa-sign-in-alt opacity-50 me-1"></i> Sign In
                                                </button>
                                                <p className="mt-3 mb-0 d-lg-flex justify-content-lg-between">
                                                    <a
                                                        className="btn btn-sm btn-alt-secondary d-block d-lg-inline-block mb-1"
                                                        href="op_auth_reminder.html"
                                                    >
                                                        <i className="fa fa-exclamation-triangle opacity-50 me-1"></i> Forgot password
                                                    </a>
                                                    <a
                                                        className="btn btn-sm btn-alt-secondary d-block d-lg-inline-block mb-1"
                                                        href="op_auth_signup.html"
                                                    >
                                                        <i className="fa fa-plus opacity-50 me-1"></i> New Account
                                                    </a>
                                                </p>
                                            </div>
                                        </form>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="hero-static col-md-6 d-none d-md-flex align-items-md-center justify-content-md-center text-md-center">
                            <div className="p-3">
                                <p className="display-4 fw-bold text-white mb-3">Welcome to the future</p>
                                <p className="fs-lg fw-semibold text-white-75 mb-0">
                                    Copyright &copy; <span data-toggle="year-copy">2024</span>
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};
export default Login;