import React, { Component } from 'react';
import ParticleBackground from './ParticleBackground';
import '../../styles/utilStyles/Background.css';
import { NavMenu } from './NavMenu';
import CanvasScale from './CanvasScale';
import "../../styles/utilStyles/BaseStyles.css";
import axios from 'axios';

axios.defaults.withCredentials = true;


export class Layout extends Component {
  static displayName = Layout.name;

  // componentDidMount() {
  //   axios.get('fcheck', {
  //     headers: {
  //       Cookie: document.cookie // Передача всех куки браузера
  //     }
  //   })
  //     .then((response) => {
  //     })
  // }

  
  render() {
    return (
      <div>
        <ParticleBackground />
        <NavMenu />
        <div className="mainContainer">
        <CanvasScale>{this.props.children}</CanvasScale>
        </div>
      </div>
    );
  }
}
