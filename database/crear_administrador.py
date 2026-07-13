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
    """Crear un administrador inicial"""
    conn = conectar_bd()
    if not conn:
        return
    
    try:
        cursor = conn.cursor()
        
        # Verificar si ya existe un administrador
        cursor.execute("SELECT COUNT(*) FROM Usuarios WHERE Rol = 'Administrador'")
        admin_count = cursor.fetchone()[0]
        
        if admin_count > 0:
            print("Ya existe al menos un administrador en la base de datos.")
            cursor.execute("SELECT CodigoUniversitario, Nombre, EmailInstitucional FROM Usuarios WHERE Rol = 'Administrador'")
            admins = cursor.fetchall()
            print("\nAdministradores existentes:")
            for admin in admins:
                print(f"  - Código: {admin[0]} | Nombre: {admin[1]} | Email: {admin[2]}")
            return
        
        # Datos del administrador
        codigo = "12345678"  # 8 dígitos
        nombre = "Administrador del Sistema"
        email = "admin@unmsm.edu.pe"  # @unmsm.edu.pe
        # Contraseña establecida en el código
        contrasena_plana = "Admin123!"  # Cambia esta contraseña según necesites
        contrasena_hash = hash_contrasena(contrasena_plana)
        
        print("=== CREANDO ADMINISTRADOR INICIAL ===")
        print(f"Código Universitario: {codigo}")
        print(f"Nombre: {nombre}")
        print(f"Email: {email}")
        print(f"Contraseña: {contrasena_plana}")
        print("=" * 50)
        
        # Insertar administrador
        cursor.execute("""
            INSERT INTO Usuarios (CodigoUniversitario, Nombre, EmailInstitucional, ContrasenaHash, Rol, Estado, FechaRegistro, FechaUltimaActualizacionContrasena)
            VALUES (?, ?, ?, ?, ?, ?, GETDATE(), GETDATE())
        """, (codigo, nombre, email, contrasena_hash, 'Administrador', 1))
        
        conn.commit()
        
        print("[OK] Administrador creado exitosamente!")
        print("\n=== INFORMACIÓN DE ACCESO ===")
        print(f"Usuario: {codigo}")
        print(f"Contraseña: {contrasena_plana}")
        print(f"Email: {email}")
        print("\n[IMPORTANTE] Guarda esta información de forma segura.")
        print("   Puedes cambiar la contraseña desde la aplicación.")
        
    except Exception as e:
        print(f"Error: {e}")
        conn.rollback()
    finally:
        conn.close()

if __name__ == "__main__":
    print("Creando administrador inicial...")
    crear_administrador()
    print("Proceso completado")
