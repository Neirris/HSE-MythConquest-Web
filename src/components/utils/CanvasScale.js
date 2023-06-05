import React, { Component } from 'react';

class CanvasScale extends Component {
  state = {
    scale: 1,
  };

  componentDidMount() {
    window.addEventListener('wheel', this.handleWheel, { passive: false });
    window.addEventListener('resize', this.handleWindowResize);
  }

  componentWillUnmount() {
    window.removeEventListener('wheel', this.handleWheel);
    window.removeEventListener('resize', this.handleWindowResize);
  }

  handleWheel = (event) => {
    if (event.ctrlKey) {
      event.preventDefault();
      const { scale } = this.state;
      const newScale = scale + event.deltaY * -0.001;

      const minScale = 0.25;
      const maxScale = 1;

      if (newScale >= minScale && newScale <= maxScale) {
        this.setState({ scale: newScale });
      } else if (newScale < minScale) {
        this.setState({ scale: minScale });
      } else {
        this.setState({ scale: maxScale });
      }
    }
  };

  handleWindowResize = () => {
    this.setState({ scale: 1 });
  };

  render() {
    const { scale } = this.state;
    const { children } = this.props;

    const contentStyle = {
      transformOrigin: 'center',
      transform: `scale(${scale})`,
    };

    return <div style={contentStyle}>{children}</div>;
  }
}

export default CanvasScale;
