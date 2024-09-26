import axios from 'axios'

axios.defaults.baseURL = 'http://localhost:5079/';
axios.defaults.headers.common['Authorization'] = localStorage.getItem('token');
