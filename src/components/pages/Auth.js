import React, { Component } from "react";
import Logo from "../../images/misc/logo.png";
import axios from "axios";
import Swal from "sweetalert2";

export class Auth extends Component {
  static displayName = Auth.name;

  state = {
    loginValue: "",
    passwordValue: "",
  };

  componentDidMount() {
    const userCookie = document.cookie
      .split(";")
      .find((cookie) => cookie.trim().startsWith("User="));
    if (userCookie) {
      const decodedCookie = decodeURIComponent(userCookie);
      const user = JSON.parse(decodedCookie.split("=")[1]);
      if (user.isAuth) {
        window.location.href = "/";
      }
    }
  }

  handleLoginInputChange = (event) => {
    const inputValue = event.target.value;
    const allowedCharacters = /^[0-9a-zA-Z!@#$%^_+&.,]+$/;

    if (inputValue === "" || allowedCharacters.test(inputValue)) {
      this.setState({ loginValue: inputValue });
    }
  };

  handlePasswordInputChange = (event) => {
    const inputValue = event.target.value;
    const allowedCharacters = /^[0-9a-zA-Z!@#$%^_+&.,]+$/;

    if (inputValue === "" || allowedCharacters.test(inputValue)) {
      this.setState({ passwordValue: inputValue });
    }
  };

  handleRegister = () => {
    const { loginValue, passwordValue } = this.state;

    if (loginValue.length < 4 || passwordValue.length < 4) {
      Swal.fire("", "Заполните данные!", "warning");
      return;
    }

    const user = {
      userId: -1,
      userName: loginValue,
      userPass: passwordValue,
      isAuth: false,
      isAdmin: false,
    };

    axios
      .post("register", user)
      .then((response) => {
        const { data } = response;
        if (response.status === 200) {
          document.cookie = `User=${JSON.stringify(data)}`;
          Swal.fire("", "Вы успешно зарегистрировались!", "success");
          setTimeout(() => {
            window.location.reload();
            window.location.href = "/";
          }, 1000);
        }
      })
      .catch((error) => {
        if (
          error.response &&
          error.response.status === 400 &&
          error.response.data === "UsersExists"
        ) {
          Swal.fire("", "Данный пользователь уже существует!", "warning");
        } else {
          Swal.fire("", "Ошибка регистрации!", "error");
        }
      });
  };

  handleLogin = () => {
    const { loginValue, passwordValue } = this.state;

    if (loginValue.length < 4 || passwordValue.length < 4) {
      Swal.fire("", "Заполните данные!", "warning");
      return;
    }

    const user = {
      userId: -1,
      userName: loginValue,
      userPass: passwordValue,
      isAuth: false,
      isAdmin: false,
    };

    axios
      .post("login", user)
      .then((response) => {
        const { data } = response;
        if (response.status === 200) {
          document.cookie = `User=${JSON.stringify(data)}`;
          Swal.fire("", "Вы успешно авторизировались!", "success");
          setTimeout(() => {
            window.location.reload();
            window.location.href = "/";
          }, 1000);
        }
      })
      .catch((error) => {
        if (
          error.response &&
          error.response.status === 400 &&
          error.response.data === "NoUsersExists"
        ) {
          Swal.fire("", "Данного пользователя не существует!", "warning");
        }
        if (
          error.response &&
          error.response.status === 400 &&
          error.response.data === "InvalidPassword"
        ) {
          Swal.fire("", "Проверьте корректность введённых данных!", "warning");
        } else {
          Swal.fire("", "Ошибка авторизации!", "error");
        }
      });
  };

  render() {
    const { loginValue, passwordValue } = this.state;

    return (
      <div>
        <div className="top">
          <div className="column align-center">
            <img src={Logo} style={{ width: "200%", height: "200%" }} alt="" />
            <h1 className="text-header-main">Myth Conquest</h1>
          </div>
        </div>
        <div className="bottom">
          <div className="column align-center">
            <input
              type="text"
              value={loginValue}
              onChange={this.handleLoginInputChange}
              placeholder="Логин"
              className="textbox-auth"
              id="login-box"
            />
            <input
              type="password"
              value={passwordValue}
              onChange={this.handlePasswordInputChange}
              placeholder="Пароль"
              className="textbox-auth"
              id="password-box"
            />

            <div className="column align-center">
              <button
                className="button-long text-40px"
                id="login-button"
                onClick={this.handleLogin}
              >
                Войти
              </button>
              <button
                className="button-long text-40px"
                id="password-button"
                onClick={this.handleRegister}
              >
                Зарегистрироваться
              </button>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
