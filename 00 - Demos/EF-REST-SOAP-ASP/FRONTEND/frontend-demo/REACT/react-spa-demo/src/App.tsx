import './App.css'
import { NavLink, Route, Routes } from 'react-router-dom';
import { CatalogPage } from './components/CatalogPage';
import { BookDetail } from './api/BookDetail';
import { About } from './api/About';

function App() {
  
  return(
    <>
      <header>
        <h1> Library </h1>
        <nav className='app-header'>
          <NavLink to={"/"}>Catalog</NavLink>
          <NavLink to={"/about"}>About</NavLink>
        </nav>
      </header>
      
      <main>
        <Routes>
          <Route path='/' element={<CatalogPage />} />
          {/* more pages... */}
          <Route path='/inventory/:sku' element={<BookDetail/>} />
          <Route path='/about' element={<About />} />

          <Route path='*' element={<p>Page not found</p>} />
        </Routes>
      </main>
    </>
  );
}

export default App
