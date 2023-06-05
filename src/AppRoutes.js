import { MainMenu } from "./components/pages/MainMenu";
import { Tower } from "./components/pages/Tower";
import { Arena } from "./components/pages/Arena";
import { Inventory } from "./components/pages/Inventory";
import { Profile } from "./components/pages/Profile";
import { Auth } from "./components/pages/Auth";
import { Shop } from "./components/pages/Shop";

const AppRoutes = [
  {
    index: true,
    element: <MainMenu />
  },
  {
    path: '/main',
    element: <MainMenu />
  },
  {
    path: '/tower',
    element: <Tower />
  },
  {
    path: '/arena',
    element: <Arena />
  },
  {
    path: '/inventory',
    element: <Inventory />
  },
  {
    path: '/profile',
    element: <Profile />
  },
  {
    path: '/auth',
    element: <Auth />
  },
  {
    path: '/shop',
    element: <Shop />
  },
  {
    path: '/login'
  },
  {
    path: '/register'
  },
  {
    path: '/exit'
  }
];

export default AppRoutes;
