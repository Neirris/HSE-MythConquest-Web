import React, { Component } from "react";
import "../../styles/utilStyles/BaseStyles.css";
import axios from "axios";
import Cookies from "js-cookie";
import Swal from "sweetalert2";

export class Tower extends Component {
  static displayName = Tower.name;

  state = {
    curr_hp: null,
  };

  constructor(props) {
    super(props);
    this.state = {
      isOpen: false,
      selectedOption: null,
      battleLog: [],
    };
    this.options = [];
    for (let i = 1; i <= 100; i++) {
      this.options.push(i);
    }
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
    axios.defaults.headers.common["currFloor"] = option;
  };

  async componentDidMount() {
    Cookies.remove('arenaLog');
    Cookies.remove('battleLog');
    await this.getHpPotions();
    window.addEventListener("beforeunload", this.handleBeforeUnload);
  }

  async getHpPotions() {
    const response = await axios.get("getpotions");
    const data = response.data;
    this.setState({ curr_hp: data });
  }

  async startTower() {
    const { selectedOption } = this.state;
    if (selectedOption !== "Выберите этаж"){
      const response = await axios.post("startTower");
      const data = response.data;
      document.cookie = `battleLog=${encodeURI(data)}`;
      this.setState({ battleLog: data });
    }

  }

  async UpdateHpPotions() {
    axios
      .post("setpotions")
      .then((response) => {
        Swal.fire("", "Здоровье восстановлено!", "success");
        setTimeout(() => {
          window.location.reload();
        }, 1000);
      })
      .catch((error) => {
        Swal.fire("", "Здоровье не восстановлено!", "warning");
        setTimeout(() => {
          window.location.reload();
        }, 1000);
      });
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
    const { isOpen, selectedOption, curr_hp, battleLog } = this.state;
    return (
      <div>
        <div className="top">
          <div className="top-square">
            <div style={{ height: "480px", overflow: "auto" }}>
              <p className="text-black text-30px" style={{ marginLeft: "5px" }}>
                {this.getCookie("battleLog") &&
                  this.getCookie("battleLog")
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
          <div className="column align-center">
            <div className="dropdown-container">
              <div className="dropdown-header" onClick={this.toggleDropdown}>
                {selectedOption || "Выберите этаж"}
              </div>
              {isOpen && (
                <ul className="dropdown-options">
                  {this.options.map((option, index) => (
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
              <button
                className="button-long text-40px"
                onClick={() => this.startTower()}
              >
                Начать прохождение
              </button>
              <button
                className="button-long text-40px"
                onClick={() => this.UpdateHpPotions()}
              >
                Восстановить здоровье
              </button>
              <h1 className="text-40px text-white">
                {" "}
                {curr_hp && curr_hp.hp_potions}/10
              </h1>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
