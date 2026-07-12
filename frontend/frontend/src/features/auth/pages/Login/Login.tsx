// src/features/auth/pages/Login/Login.tsx
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Eye, EyeOff } from "lucide-react";
import "./Login.css";
import { loginWithEmail } from "../../../../api/auth";
import { useAuth } from "../../../../hooks/useAuth";
import type { Usuario } from "../../../../contexts/AuthContextTypes";

const Login: React.FC = () => {
    const { login, usuario } = useAuth();
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [mostrarPassword, setMostrarPassword] = useState(false);
    const [mensaje, setMensaje] = useState("");
    const [errorConexion, setErrorConexion] = useState(false);
    const [mostrarModalMensaje, setMostrarModalMensaje] = useState(false);
    const [tipoMensaje, setTipoMensaje] = useState<"success" | "error" | "warning">("success");
    const [tituloError, setTituloError] = useState("");

    // Navegar automáticamente cuando el usuario se autentica
    useEffect(() => {
        if (usuario && mostrarModalMensaje && tipoMensaje === "success") {
            const timer = setTimeout(() => {
                navigate("/dashboard", { replace: true });
            }, 1500); // Esperar 1.5 segundos para que el usuario vea el mensaje
            return () => clearTimeout(timer);
        }
    }, [usuario, mostrarModalMensaje, tipoMensaje, navigate]);

    const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        let value = e.target.value;
        // Remover @unmsm.edu.pe si el usuario lo escribió
        value = value.replace(/@unmsm\.edu\.pe/g, '');
        // Remover cualquier @ que pueda haber escrito
        value = value.replace('@', '');
        setEmail(value);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setMensaje("");

        // Concatenar automáticamente el dominio si no está presente
        const emailCompleto = email.trim() ? `${email.trim()}@unmsm.edu.pe` : email;

        try {
            const data = await loginWithEmail(emailCompleto, password);
            // Mapear la respuesta del backend a la estructura Usuario
            const usuario: Usuario = {
                usuarioID: data.usuarioID,
                nombre: data.nombreUsuario || data.nombre,
                emailInstitucional: emailCompleto,
                codigoUniversitario: data.codigoUniversitario || '',
                rol: data.rol
            };
            setMensaje("¡Bienvenido " + usuario.nombre + "!");
            setTipoMensaje("success");
            setTituloError("");
            setMostrarModalMensaje(true);
            login(usuario);
            setErrorConexion(false);
        } catch (err: unknown) {
            console.error('Error en login:', err);
            
            // Verificar si es un error de Axios con response
            const isAxiosError = err && 
                typeof err === 'object' && 
                'response' in err && 
                err.response && 
                typeof err.response === 'object' && 
                'status' in err.response;
            
            if (isAxiosError) {
                const status = (err.response as { status: number }).status;
                
                if (status === 401) {
                    // Error 401: Correo no registrado
                    setTituloError("Correo No Registrado");
                    setMensaje("No existe una cuenta registrada con este correo electrónico. Verifica el email o regístrate si eres nuevo.");
                    setTipoMensaje("error");
                    setMostrarModalMensaje(true);
                } else if (status === 500) {
                    // Error 500: Correo registrado pero contraseña incorrecta
                    setTituloError("Contraseña Incorrecta");
                    setMensaje("La contraseña ingresada no es correcta. Verifica tu contraseña e intenta nuevamente.");
                    setTipoMensaje("error");
                    setMostrarModalMensaje(true);
                } else if (status === 404) {
                    // Error 404: Usuario no encontrado
                    setTituloError("Correo No Registrado");
                    setMensaje("No existe una cuenta asociada a este email. Verifica el correo o regístrate si eres nuevo.");
                    setTipoMensaje("error");
                    setMostrarModalMensaje(true);
                } else {
                    // Otros errores del servidor
                    setTituloError("Error de Conexión");
                    setMensaje("No se pudo conectar con el servidor. Verifica tu conexión a internet.");
                    setTipoMensaje("warning");
                    setMostrarModalMensaje(true);
                }
            } else {
                // Error de red o conexión
                setTituloError("Error de Conexión");
                setMensaje("No se pudo conectar con el servidor. Verifica tu conexión a internet.");
                setTipoMensaje("warning");
                setMostrarModalMensaje(true);
            }
        }
    };

    return (
        <div className="login-page">
            {/* Overlay animado */}
            {errorConexion && (
                <div className="login-modal-overlay">
                    <div className="login-modal-container">
                        <div className="login-modal-header">
                            <h3 className="login-modal-title">Error de Conexión</h3>
                            <button
                                className="login-modal-close-button"
                                onClick={() => setErrorConexion(false)}
                            >
                                ×
                            </button>
                        </div>
                        <div className="login-modal-content">
                            <div className="login-modal-icon warning">⚠️</div>
                            <div className="login-modal-message">
                                <p>No se pudo conectar con el servidor</p>
                                <p>Verifica tu conexión a internet o contacta al administrador del sistema</p>
                            </div>
                        </div>
                        <div className="login-modal-footer">
                            <button
                                className="login-modal-button warning"
                                onClick={() => setErrorConexion(false)}
                            >
                                Entendido
                            </button>
                        </div>
                    </div>
                </div>
            )}

            <div className="stage">
                <div className="blob a"></div>
                <div className="blob b"></div>
            </div>

            <main className="wrap">
                <section className="hero animate-fade-in">
                    <div className="brand">
                        <div className="logo animate-pop">📘</div>
                        <div>
                            <h1 className="title">Biblioteca FISI · UNMSM</h1>
                            <p className="subtitle">
                                Accede a tus préstamos y recursos digitales
                            </p>
                        </div>
                    </div>
                    <div className="features">
                        <div className="f">
                            <p><b>+50,000 títulos</b> en catálogo digital y físico</p>
                        </div>
                        <div className="f">
                            <p><b>Renovación en línea</b> de tus préstamos</p>
                        </div>
                        <div className="f">
                            <p><b>Acceso académico</b> a tesis y material de investigación</p>
                        </div>
                    </div>
                </section>

                <aside className="login-card animate-slide-up">
                    <h2>Inicia sesión</h2>
                    <p>Usa tu correo institucional o accede con Google</p>
                    <form onSubmit={handleSubmit}>
                        <div>
                            <label htmlFor="email">Email Institucional</label>
                            <div className="login-input login-input-email">
                                <input
                                    type="text"
                                    id="email"
                                    placeholder="tu"
                                    required
                                    value={email}
                                    onChange={handleEmailChange}
                                    style={{ 
                                        color: '#1f2937', 
                                        WebkitTextFillColor: '#1f2937'
                                    }}
                                />
                                <span>
                                    @unmsm.edu.pe
                                </span>
                            </div>
                        </div>
                        <div>
                            <label htmlFor="password">Contraseña</label>
                            <div className="login-input login-input-password">
                                <input
                                    type={mostrarPassword ? "text" : "password"}
                                    id="password"
                                    placeholder="••••••••"
                                    required
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    style={{ color: '#1f2937', WebkitTextFillColor: '#1f2937' }}
                                />
                                <button
                                    type="button"
                                    className="password-toggle-button"
                                    onClick={() => setMostrarPassword(!mostrarPassword)}
                                    aria-label={mostrarPassword ? "Ocultar contraseña" : "Mostrar contraseña"}
                                >
                                    {mostrarPassword ? (
                                        <EyeOff size={20} color="#6b7280" />
                                    ) : (
                                        <Eye size={20} color="#6b7280" />
                                    )}
                                </button>
                            </div>
                        </div>
                        <button className="btn btn--primary" type="submit">
                            Entrar
                        </button>

                    </form>

                    <div className="auth-links">
                        <p>¿No tienes cuenta?</p>
                        <button 
                            type="button" 
                            className="link-button"
                            onClick={() => navigate('/registro')}
                        >
                            Regístrate aquí
                        </button>
                    </div>
                </aside>
            </main>

            {/* Modal de Éxito */}
            {mostrarModalMensaje && tipoMensaje === "success" && (
                <div className="login-modal-overlay">
                    <div className="login-modal-container">
                        <div className="login-modal-header">
                            <h3 className="login-modal-title">¡Bienvenido!</h3>
                            <button
                                className="login-modal-close-button"
                                onClick={() => setMostrarModalMensaje(false)}
                            >
                                ×
                            </button>
                        </div>
                        <div className="login-modal-content">
                            <div className="login-modal-icon success">✅</div>
                            <div className="login-modal-message">
                                <p>{mensaje}</p>
                            </div>
                        </div>
                        <div className="login-modal-footer">
                            <button 
                                className="login-modal-button success"
                                onClick={() => {
                                    setMostrarModalMensaje(false);
                                    navigate("/dashboard", { replace: true });
                                }}
                            >
                                Continuar
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Modal de Error */}
            {mostrarModalMensaje && (tipoMensaje === "error" || tipoMensaje === "warning") && (
                <div className="login-modal-overlay">
                    <div className="login-modal-container">
                        <div className="login-modal-header">
                            <h3 className="login-modal-title">{tituloError}</h3>
                            <button
                                className="login-modal-close-button"
                                onClick={() => setMostrarModalMensaje(false)}
                            >
                                ×
                            </button>
                        </div>
                        <div className="login-modal-content">
                            <div className={`login-modal-icon ${tipoMensaje}`}>
                                {tipoMensaje === "error" ? "❌" : "⚠️"}
                            </div>
                            <div className="login-modal-message">
                                <p>{mensaje}</p>
                            </div>
                        </div>
                        <div className="login-modal-footer">
                            <button 
                                className={`login-modal-button ${tipoMensaje}`}
                                onClick={() => setMostrarModalMensaje(false)}
                            >
                                Entendido
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Login;
