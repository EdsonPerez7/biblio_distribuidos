#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para crear un administrador inicial
NOTA: La contraseña está establecida en el código (línea 88).
      Cambia la variable 'contrasena_plana' si necesitas una contraseña diferente.
"""

import pyodbc
import hashlib

def conectar_bd():
    """Conectar a la base de datos con múltiples intentos"""
    import os
    
    # Intentar leer contraseña del archivo .env
    password = None
    env_paths = ['../backend/.env', 'backend/.env', '.env']
    for p in env_paths:
        if os.path.exists(p):
            try:
                with open(p, 'r') as f:
                    for line in f:
                        if line.startswith('MSSQL_SA_PASSWORD='):
                            password = line.split('=', 1)[1].strip()
                            break
            except Exception:
                pass
        if password:
            break

    # Lista de posibles configuraciones de servidor
    servidores = [
        'localhost',  # SQL Server por defecto
        'localhost,1433', # Puerto Docker mapeado
        '127.0.0.1,1433',
        'localhost\\SQLEXPRESS',  # SQL Server Express
        '.',  # Instancia local
    ]
    
    # Intentar con cada configuración de servidor
    for servidor in servidores:
        # Si tenemos contraseña, probar primero con autenticación SQL Server
        if password:
            for driver in ['{ODBC Driver 18 for SQL Server}', '{ODBC Driver 17 for SQL Server}']:
                try:
                    print(f"Intentando conectar a: {servidor} con sa usando {driver}...")
                    conn = pyodbc.connect(
                        f'DRIVER={driver};'
                        f'SERVER={servidor};'
                        'DATABASE=BibliotecaFISI;'
                        'UID=sa;'
                        f'PWD={password};'
                        'TrustServerCertificate=yes;'
                        'Connection Timeout=5;'
                    )
                    print(f"✅ Conexión exitosa a: {servidor}")
                    return conn
                except pyodbc.Error:
                    continue

        # Intentar con Trusted Connection (Autenticación de Windows)
        try:
            print(f"Intentando conectar a: {servidor} con Trusted Connection...")
            conn = pyodbc.connect(
                'DRIVER={ODBC Driver 17 for SQL Server};'
                f'SERVER={servidor};'
                'DATABASE=BibliotecaFISI;'
                'Trusted_Connection=yes;'
                'Connection Timeout=5;'
            )
            print(f"✅ Conexión exitosa a: {servidor}")
            return conn
        except pyodbc.Error as e:
            continue
        except Exception as e:
            continue
    
    # Si todos los intentos fallaron, mostrar mensaje de ayuda
    print("\n❌ Error: No se pudo conectar a SQL Server con ninguna configuración.")
    print("\nPosibles soluciones:")
    print("1. Verifica que SQL Server esté ejecutándose en Docker (sqlserver_biblioteca)")
    print("2. Asegúrate de tener instalado un controlador ODBC para SQL Server:")
    print("   En Ubuntu: sudo apt-get install msodbcsql17 o msodbcsql18")
    print("3. Verifica que la base de datos 'BibliotecaFISI' exista en el contenedor")
    return None

def hash_contrasena(contrasena):
    """Crear hash de la contraseña usando SHA-256"""
    return hashlib.sha256(contrasena.encode('utf-8')).hexdigest()

def crear_administrador():
    """Crear un administrador y un usuario comun inicial"""
    conn = conectar_bd()
    if not conn:
        return
    
    try:
        cursor = conn.cursor()
        
        # Eliminar usuarios anteriores si existen para evitar conflictos
        cursor.execute("DELETE FROM Usuarios WHERE EmailInstitucional IN ('admin@unmsm.edu.pe', 'usuario1@unmsm.edu.pe')")
        conn.commit()

        # 1. Crear Administrador
        codigo_admin = "11111111"
        nombre_admin = "Administrador del Sistema"
        email_admin = "admin@unmsm.edu.pe"
        contrasena_admin = "admin"
        hash_admin = hash_contrasena(contrasena_admin)

        print("=== CREANDO ADMINISTRADOR INICIAL ===")
        print(f"Código Universitario: {codigo_admin}")
        print(f"Nombre: {nombre_admin}")
        print(f"Email: {email_admin}")
        print(f"Contraseña: {contrasena_admin}")
        
        cursor.execute("""
            INSERT INTO Usuarios (CodigoUniversitario, Nombre, EmailInstitucional, ContrasenaHash, Rol, Estado, FechaRegistro, FechaUltimaActualizacionContrasena)
            VALUES (?, ?, ?, ?, ?, ?, GETDATE(), GETDATE())
        """, (codigo_admin, nombre_admin, email_admin, hash_admin, 'Administrador', 1))
        
        # 2. Crear Usuario Comun
        codigo_user = "22222222"
        nombre_user = "Usuario Comun"
        email_user = "usuario1@unmsm.edu.pe"
        contrasena_user = "asdf"
        hash_user = hash_contrasena(contrasena_user)

        print("\n=== CREANDO USUARIO COMUN INICIAL ===")
        print(f"Código Universitario: {codigo_user}")
        print(f"Nombre: {nombre_user}")
        print(f"Email: {email_user}")
        print(f"Contraseña: {contrasena_user}")
        print("=" * 50)

        cursor.execute("""
            INSERT INTO Usuarios (CodigoUniversitario, Nombre, EmailInstitucional, ContrasenaHash, Rol, Estado, FechaRegistro, FechaUltimaActualizacionContrasena)
            VALUES (?, ?, ?, ?, ?, ?, GETDATE(), GETDATE())
        """, (codigo_user, nombre_user, email_user, hash_user, 'Estudiante', 1))

        conn.commit()
        
        print("\n[OK] Usuarios de prueba creados exitosamente!")
        print("\n=== CREDENCIALES DE ACCESO ===")
        print("1. ROL ADMINISTRADOR:")
        print("   Usuario/Email en login: admin (se autocompleta como admin@unmsm.edu.pe)")
        print("   Contraseña: admin")
        print("\n2. ROL ESTUDIANTE:")
        print("   Usuario/Email en login: usuario1 (se autocompleta como usuario1@unmsm.edu.pe)")
        print("   Contraseña: asdf")
        
    except Exception as e:
        print(f"Error: {e}")
        conn.rollback()
    finally:
        conn.close()

if __name__ == "__main__":
    print("Creando usuarios iniciales...")
    crear_administrador()
    print("Proceso completado")
