import React, { Component } from "react";
import { Navbar, NavItem, NavLink } from "reactstrap";
import { Link } from "react-router-dom";
import axios from 'axios';
import "../../styles/utilStyles/NavMenu.css";

export class NavMenu extends Component {
  static displayName = NavMenu.name;
  constructor(props) {
    super(props);
    this.state = {
      isMenuOpen: false,
    };
    this.menuRef = React.createRef();
  }

  componentDidMount() {
    document.addEventListener("mousemove", this.handleMouseMove);
  }

  componentWillUnmount() {
    document.removeEventListener("mousemove", this.handleMouseMove);
  }

  handleMouseMove = (event) => {
    const { isMenuOpen } = this.state;
    const menuWidth = this.menuRef.current.offsetWidth;
    const screenWidth = window.innerWidth;
    const mouseX = event.pageX;

    if (!isMenuOpen && mouseX > screenWidth - menuWidth) {
      this.setState({ isMenuOpen: true });
    } else if (isMenuOpen && mouseX < screenWidth - menuWidth) {
      this.setState({ isMenuOpen: false });
    }
  };

  handleClick = () => {
    this.setState((prevState) => ({
      isMenuOpen: !prevState.isMenuOpen,
    }));
  };

  handleLogout = () => {
    axios.post('logout')
  };
  

  render() {
    const { isMenuOpen } = this.state;

    return (
      <div
        className={`nav-menu ${isMenuOpen ? "open" : ""}`}
        ref={this.menuRef}
      >
        <Navbar className="nav-menu-content" container light>
          <ul className="navbar-nav flex-grow">
            <NavItem>
              <NavLink tag={Link} className="text-dark" to="/">
                Меню
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink tag={Link} className="text-dark" to="/tower">
                Башня
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink tag={Link} className="text-dark" to="/arena">
                Арена
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink tag={Link} className="text-dark" to="/shop">
                Магазин
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink tag={Link} className="text-dark" to="/inventory">
                Инвентарь
              </NavLink>
            </NavItem>
            <NavItem>
              <NavLink tag={Link} className="text-dark" to="/profile">
                Профиль
              </NavLink>
            </NavItem>
            <NavItem className="empty" />
            <NavItem className="empty" />
            <NavItem className="empty" />
            <NavItem>
              <NavLink tag={Link} className="text-dark" to="/auth" onClick={this.handleLogout}>
                Выйти
              </NavLink>
            </NavItem>
          </ul>
        </Navbar>
      </div>
    );
  }
}
