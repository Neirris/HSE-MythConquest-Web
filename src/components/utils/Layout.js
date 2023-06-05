import React, { Component } from 'react';
import ParticleBackground from './ParticleBackground';
import '../../styles/utilStyles/Background.css';
import { NavMenu } from './NavMenu';
import CanvasScale from './CanvasScale';
import "../../styles/utilStyles/BaseStyles.css";

export class Layout extends Component {
  static displayName = Layout.name;

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
