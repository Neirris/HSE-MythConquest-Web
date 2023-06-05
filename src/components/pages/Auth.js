import React, { Component } from 'react';
import Logo from '../../images/misc/logo.png';

export class Auth extends Component {
    static displayName = Auth.name;
  
    state = {
      loginValue: '',
      passwordValue: '',
    };
  
    handleLoginInputChange = (event) => {
      const inputValue = event.target.value;
      const allowedCharacters = /^[0-9a-zA-Z!@#$%^_+&.,]+$/;
    
      if (inputValue === '' || allowedCharacters.test(inputValue)) {
        this.setState({ loginValue: inputValue });
      }
    };
    
    handlePasswordInputChange = (event) => {
      const inputValue = event.target.value;
      const allowedCharacters = /^[0-9a-zA-Z!@#$%^_+&.,]+$/;
    
      if (inputValue === '' || allowedCharacters.test(inputValue)) {
        this.setState({ passwordValue: inputValue });
      }
    };
  
  
    render() {
      const { loginValue, passwordValue } = this.state;
  
      return (
        <div>
          <div className="top">
            <div className="column align-center">
              <img src={Logo} style={{ width: '200%', height: '200%' }} alt=''/>
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
                className='textbox-auth'
                id='login-box'
              />
              <input
                type="text"
                value={passwordValue}
                onChange={this.handlePasswordInputChange}
                placeholder="Пароль"
                className='textbox-auth'
                id='password-box'
              />
               
              <div className="column align-center">
                <button className="button-long text-40px" id = "login-button">Войти</button>
                <button className="button-long text-40px" id = "password-button">Зарегистрироваться</button>
              </div>
            </div>
          </div>
        </div>
      );
    }
  }