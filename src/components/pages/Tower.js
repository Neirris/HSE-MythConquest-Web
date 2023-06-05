import React, { Component } from "react";
import "../../styles/utilStyles/BaseStyles.css";

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
  };

  async componentDidMount() {
    await this.getHpPotions();
}

  async getHpPotions() {
  const response = await fetch('getpotions');
  const data = await response.json();
  this.setState({ curr_hp: data });
  }

  render() {
    const { isOpen, selectedOption, curr_hp} = this.state;
    return (
      <div>
        <div className="top">
          <div className="top-square">
            <div style={{ height: "480px", overflow: "auto" }}>
              <p
                className="text-black text-30px"
                style={{ marginLeft: "5px" }}
              ></p>
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
              <button className="button-long text-40px">
                Начать прохождение
              </button>
              <button className="button-long text-40px">
                Восстановить здоровье
              </button>
              <h1 className="text-40px text-white"> {curr_hp && (curr_hp.hp_potions)}/10</h1>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
