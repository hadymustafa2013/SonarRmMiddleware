<template>
    <div >
        <nav class="navbar navbar-expand-lg navbar-dark bg-primary fixed-top">
            <div class="container-fluid">
                <div class="collapse navbar-collapse" id="navbarTogglerDemo03">
                    <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                        <li class="nav-item">
                            <router-link class="nav-link active" to="/">CreditLens Middleware</router-link>
                        </li>
                        <li class="nav-item">
                            <router-link class="nav-link" to="/">Home</router-link>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#">Services</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="#">Contact</a>
                        </li>
                    </ul>

                    <ul v-if="user" class="navbar-nav">
                        <li class="nav-item dropdown">
                            <span class="nav-link active dropdown-toggle"
                                  id="navbarDarkDropdownMenuLink"
                                  role="button"
                                  data-bs-toggle="dropdown"
                                  aria-expanded="false">
                                {{ user }}
                            </span>
                            <ul class="dropdown-menu dropdown-menu-dark dropdown-menu-end"
                                aria-labelledby="navbarDarkDropdownMenuLink">
                                <li><a class="dropdown-item" href="#">About Middleware</a></li>
                                <hr class="my-2" />
                                <li class="nav-item">
                                    <a class="dropdown-item" href="javascipt:void(0)" @click="logout" to="/login">Logout</a>
                                </li>
                            </ul>
                        </li>
                    </ul>
                    <ul class="navbar-nav" v-if="!user">
                        <li class="nav-item">
                            <router-link class="nav-link active" to="/login">Login</router-link>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </div>

</template>

<script>
    import { mapGetters } from 'vuex';
    import axios from 'axios'
    export default {
        name: 'Navbar',
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

