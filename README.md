# Game Serverless usando Google Firebase

En este proyecto individual se trabajó con los paquete de Google Firebase para mantener y controlar la base de datos de los usuarios que entren y se registren para este juego simple en particular, el video juego no es lo importante en el repositorio si no el cómo podemos administrar y controlar flujos de información en el aplicativo .apk de este juego únicamente para dispositivos Android y así llevar control de autenticaciones y nombres de usuarios o nicknames y puntajes de marcador dentro del juego. 

![image](https://github.com/Michikatsu0/GameServerless/assets/68073260/ba6730db-361d-4da5-a0f7-686c625b8fab)

## Paquetes utilizados del SDK de Google Firebase:

- FirebaseAuth: Control de inicio de sesión y atenticaciones de usuarios habilitado que sea posible el registrarse e incluso entrar a jugar como invitado
- RealtimeDatabase: Control de información dentro de la base de datos guardado en el .json del servidor.

## Registro, Inicio de Sesión y Manejo de Errores

El programa cuenta con un inicio de sesión común para usuarios que lleguen, es posible realizar un registro con proveedor de email y adjuntar un nombre de usuario y contraseña al mismo.
La base de datos de Firebase me permite loggear a cuantos usuarios quiera para administrar y tomar registro de las personas que normalmente entran al juego a jugar por medio del registro.

![image](https://github.com/Michikatsu0/GameServerless/assets/68073260/99da421c-c7e3-4204-aefb-f89df0d80064)

Los diferentes inputfields para llenar la informacion de email, usuario y contraseña del inicio de sesión y el registro tiene restricciones de parametros especificos para cumplir, si alguno no se cumple se notifica en la ui de la app, por medio de codigo.

![image](https://github.com/Michikatsu0/GameServerless/assets/68073260/071c50b0-a356-4b5d-b8ce-fe94858c14d0)

![image](https://github.com/Michikatsu0/GameServerless/assets/68073260/d896ee2f-b0ae-4848-95c6-8a27a0c6fe46)

![image](https://github.com/Michikatsu0/GameServerless/assets/68073260/2b97a395-573f-41e8-b35e-9c571c94dacf)

## Recuperación de contraseña

El sistema permite a los registrados con proveedor de email que puedan enviar al correo con el que se registraron, a este llega un link que permite cambiar la contraseña del usuario sin exponerla en la base de datos.

![image](https://github.com/Michikatsu0/GameServerless/assets/68073260/15708d1e-d420-4c86-a6d5-df4c2fff2276)

## Tabla de puntajes y Cerrar sesión de Usuais

Se implemente un menu con acceso a tablas de puntaje de los scores mas altos en el juego y tambien para cerrar sesión de la cuenta actual (si entras como anonimo al cerrar sesion se borran todos los datos para no llenar el server de informacion residual o basura).

![image](https://github.com/Michikatsu0/GameServerless/assets/68073260/ddb57163-3fab-47fc-bd73-85db71365a1a)

Se implementa sistema de Score o tabla de puntaje para los jugadores en tiempo real, la tabla se actualiza al entrar en ella y esta se "descargada" de los datos que hayan en el apartado "score" del json de los jugadores en la base de datos y se organizan de mayor a menor usando como base un diccionario en el código.

![image](https://github.com/Michikatsu0/GameServerless/assets/68073260/312906d0-e5ad-461a-adea-2f5d52358dc7)

## Iniciar sesión como invitado

Se tiene implementado un sistema de inicio de sesión para anónimos que permite unirse al juego sin un registro convencional y de esa forma entrar más directamente a probar el juego. El sistema te registra con un Token al igual que con los registrados por proveedor de email, solo que al cerrar sesión estos datos son borrados de la base de datos.

![image](https://github.com/Michikatsu0/GameServerless/assets/68073260/da346549-735f-4228-903e-385b7759f298)

