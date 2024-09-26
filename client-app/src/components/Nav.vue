<template>
<nav class="navbar navbar-expand-lg navbar-light bg-light">
    <div class="container justify-content-between">
        <a class="navbar-brand" href="#">Middleware</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarNav">
            <ul class="navbar-nav">
                <li class="nav-item">
                    <router-link class="nav-link" to="/">Home</router-link>
                </li>
                <li class="nav-item">
                    <router-link class="nav-link" to="/">About</router-link>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#">Services</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" href="#">Contact</a>
                </li>
            </ul>
        </div>
        <ul class="navbar-nav" v-if="!user">
            <li class="nav-item">
                <router-link class="nav-link" to="/login">Login</router-link>
            </li>
        </ul>
        <ul class="navbar-nav" v-if="user">
            <li class="nav-item">
                <span class="navbar-text">Welcome, {{ user }}</span>
            </li>
            <li class="nav-item">
                <a href="javascipt:void(0)" @click="logout" class="nav-link" to="/login">Logout</a>
            </li>
        </ul>
    </div>
</nav>
</template>

<script>
    import { mapGetters } from 'vuex';
    import axios from 'axios'
    export default {
        name: 'Nav',
        methods: {
            logout() {
                localStorage.removeItem("token");
                localStorage.removeItem("UserName");
                axios.defaults.headers.common['Authorization'] = localStorage.getItem('token');
                this.$store.dispatch('user', null);
                this.$router.push('/');
            }
        },
        computed: {
            ...mapGetters(['user'])
        }
    };
</script>

