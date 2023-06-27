import React, { Component } from "react";
import "../../styles/utilStyles/BaseStyles.css";
import axios from "axios";
import Cookies from "js-cookie";

export class Arena extends Component {
  static displayName = Arena.name;

  state = {
    users: "",
  };

  constructor(props) {
    super(props);
    this.state = {
      isOpen: false,
      selectedOption: null,
      arenaLog: [],
    };
    this.options = [];
  }

  toggleDropdown = () => {
    this.setState((prevState) => ({
      isOpen: !prevState.isOpen,
    }));
  };

  handleOptionSelect = (option) => {
    this.setState({
      selectedOption: option,
      isOpen: false,
    });
    axios.defaults.headers.common["currEnemyPlayer"] = option;
  };

  async getUsers() {
    const response = await fetch("/getusers");
    const data = await response.json();
    this.setState({ users: data });
  }

  componentDidMount() {
    this.getUsers();
    Cookies.remove('arenaLog');
    Cookies.remove('battleLog');
    window.addEventListener("beforeunload", this.handleBeforeUnload);
  }

  async startArena() {
    const { selectedOption } = this.state;
    if (selectedOption !== "Выберите игрока"){
      const response = await axios.post("startArena");
      const data = response.data;
      document.cookie = `arenaLog=${encodeURI(data)}`;
      this.setState({ arenaLog: data });
    }
  }

  getCookie(name) {
    const cookies = document.cookie.split(";").map((cookie) => cookie.trim());
    for (let i = 0; i < cookies.length; i++) {
      const cookie = cookies[i];
      if (cookie.startsWith(name + "=")) {
        return decodeURI(cookie.substring(name.length + 1));
      }
    }
    return null;
  }

  componentWillUnmount() {
    window.removeEventListener("beforeunload", this.handleBeforeUnload);
    Cookies.remove('arenaLog');
    Cookies.remove('battleLog');
  }

  handleBeforeUnload = () => {
    Cookies.remove('arenaLog');
    Cookies.remove('battleLog');
  };


  render() {
    const { isOpen, selectedOption, users } = this.state;
    return (
      <div>
        <div className="top">
          <div className="top-square">
            <div style={{ height: "480px", overflow: "auto" }}>
              <p className="text-black text-30px" style={{ marginLeft: "5px" }}>
                {this.getCookie("arenaLog") &&
                  this.getCookie("arenaLog")
                    .split(",")
                    .map((item, index) => (
                      <span key={index}>
                        {item}
                        <br />
                      </span>
                    ))}
              </p>
            </div>
          </div>
        </div>
        <div className="bottom">
          <div className="button-container">
            <div className="dropdown-container">
              <div className="dropdown-header" onClick={this.toggleDropdown}>
                {selectedOption || "Выберите игрока"}
              </div>
              {isOpen && (
                <ul className="dropdown-options">
                  {users.map((option, index) => (
                    <li
                      key={index}
                      onClick={() => this.handleOptionSelect(option)}
                    >
                      {option}
                    </li>
                  ))}
                </ul>
              )}
            </div>
            <div className="column align-center">
              <button className="button-long text-40px" onClick={() => this.startArena()}>Сразиться</button>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
