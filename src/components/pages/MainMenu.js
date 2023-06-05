import React, { Component } from "react";
import Logo from "../../images/misc/logo.png";
import { Link } from "react-router-dom";

import "../../styles/utilStyles/BaseStyles.css";
import "../../styles/pageStyles/MainMenu.css";

export class MainMenu extends Component {
  static displayName = MainMenu.name;

  render() {
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
            <div className="row align-center">
            <Link to="/tower" style={{ width: "auto"}}>
              <button className="button-long text-50px">Башня</button>
            </Link>
            <Link to="/arena" style={{ width: "auto"}}>
              <button className="button-long text-50px">Арена</button>
            </Link>
            </div>
            <div className="row align-center">
            <Link to="/shop" style={{ width: "auto"}}>
              <button className="button-short text-50px">Магазин</button>
            </Link>
            <Link to="/inventory" style={{ width: "auto"}}>
              <button className="button-short text-50px">Инвентарь</button>
            </Link>
            <Link to="/profile" style={{ width: "auto"}}>
              <button className="button-short text-50px">Профиль</button>
            </Link>
              
              
              
            </div>
          </div>
        </div>
      </div>
    );
  }
}
