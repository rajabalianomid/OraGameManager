import { observer } from 'mobx-react-lite'
import PageFooter from '../features/PageFooter'
import PageHeader from '../features/PageHeader'
import Slidebar from '../features/Sidebar'
import SideOverlay from '../features/SideOverlay'
import { useStore } from '../Store'
import './App.css'
import { useEffect, useState } from 'react'
import { Outlet } from 'react-router-dom'

function App() {

  const { mainStore } = useStore();
  const [isMobile, setIsMobile] = useState(false);

  const updateScreenSize = () => {
    setIsMobile(window.innerWidth < 1024);
    if (window.innerWidth < 1024) {
      mainStore.setMenuClose(true);
    }
  };

  useEffect(() => {
    if (mainStore.withOutSlider)
      mainStore.setMenuClose(true);
    updateScreenSize();
    window.addEventListener('resize', updateScreenSize);
    return () => window.removeEventListener('resize', updateScreenSize);
  }, []);

  return (

    <div id="page-container" className={`sidebar-dark enable-page-overlay side-scroll page-header-fixed main-content-narrow side-trans-enabled ${(!isMobile && !mainStore.isMenuClose && !mainStore.withOutSlider) ? "sidebar-o" : ""} ${isMobile && !mainStore.isMenuClose && !mainStore.withOutSlider ? "sidebar-o-xs" : ""}`}>
      {!mainStore.withOutSlider && <SideOverlay />}
      {!mainStore.withOutSlider && <Slidebar />}
      <PageHeader />
      <Outlet />
      <PageFooter />
    </div>

  )
}

export default observer(App);
