import React, { Component } from "react";
import "../../styles/utilStyles/BaseStyles.css";

export class Arena extends Component {
  static displayName = Arena.name;

  state = {
    users: '',
  };

  constructor(props) {
    super(props);
    this.state = {
      isOpen: false,
      selectedOption: null,
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
  };

  async getUsers() {
    const response = await fetch("/getusers");
    const data = await response.json();
    this.setState({ users: data });
  }

  componentDidMount() {
    this.getUsers();
  }


  render() {
    const { isOpen, selectedOption, users} = this.state;
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
              <button className="button-long text-40px">Сразиться</button>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
