import { createMemoryHistory, createRouter } from 'vue-router'


const routes = [
    { path: '/', component: () => import('./components/Home.vue') },
    { path: '/login', component: () => import('./components/Login.vue') }
]

const router = createRouter({
    history: createMemoryHistory(),
    routes,
})

export default router