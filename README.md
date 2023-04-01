# RedeSocial Infnet 🎉🌐

Bem-vindo(a) à documentação da API da RedeSocial Infnet! Aqui você encontrará todas as informações necessárias para começar a usar nossa API de maneira eficiente. 😄

A RedeSocial Infnet é um projeto desenvolvido por alunos do curso de Análise e Desenvolvimento de Sistemas da faculdade Infnet, fundada em 1994.

![GitHub top language](https://img.shields.io/github/languages/top/usuario/repositorio)
![GitHub last commit](https://img.shields.io/github/last-commit/usuario/repositorio)
![GitHub stars](https://img.shields.io/github/stars/usuario/repositorio?style=social)

## Endpoints 🚀

Aqui estão os principais endpoints que você pode utilizar para interagir com nossa API.

### Autenticação 🔐

- `POST /api/Auth/Cadastro`: Registre um novo usuário na plataforma.
- `POST /api/Auth/Login`: Faça login com suas credenciais de usuário.
- `GET /api/Auth/perfil/{userName}`: Obtenha informações do perfil do usuário.
- `PUT /api/Auth/Editar/{userName}`: Atualize informações do perfil do usuário.

### Postagens 📝

- `GET /api/Post`: Obtenha todas as postagens.
- `GET /api/Post/usuario/{userName}`: Obtenha todas as postagens de um usuário específico.
- `GET /api/Post/{id}`: Obtenha detalhes de uma postagem específica.
- `POST /api/Post`: Crie uma nova postagem.
- `DELETE /api/Post/{id}`: Exclua uma postagem específica.

## Exemplos de requisições 🧪

Aqui estão alguns exemplos de como usar os endpoints mencionados acima.

### Autenticação 🔐

#### Cadastro

```http
POST /api/Auth/Cadastro
Content-Type: application/json

{
"UserName": "johndoe",
"Email": "john.doe@example.com",
"Password": "supersecretpassword",
"Localidade": "Rio de Janeiro, Brasil",
"AreaMigracao": "Desenvolvedor Full-Stack",
"FotoPerfilByte": null
}
```


#### Login

```http
POST /api/Auth/Login
Content-Type: application/json
{
    "UserName": "johndoe",
    "Password": "supersecretpassword"
}
```


### Postagens 📝

#### Criar postagem

```http
POST /api/Post
Content-Type: application/json
Authorization: Bearer your_jwt_token
{
    "Titulo": "Minha primeira postagem",
    "Conteudo": "Olá, mundo! Esta é a minha primeira postagem na RedeSocial Infnet.",
    "Imagem": null
}
```


## Próximos passos 🎯

Agora que você está familiarizado com os principais endpoints e exemplos de requisições, você está pronto para começar a explorar e interagir com a RedeSocial Infnet API! Divirta-se e não hesite em entrar em contato conosco caso precise de mais informações ou ajuda. 🤓🎉

[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/built-with-love.svg)](https://forthebadge.com)

