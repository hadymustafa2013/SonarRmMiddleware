<template>
    <div class="container">
        <div class="row justify-content-center align-items-center vh-100 mt-5">
            <div class="col-md-6">
                <div class="card">
                    <div class="card-body">
                        <h3 class="card-title text-center mb-4">Login</h3>
                        <form @submit.prevent="login">
                            <div class="mb-3">
                                <label for="username">Username:</label>
                                <input type="text" class="form-control" id="username" v-model="username" placeholder="username" />
                            </div>
                            <div class="mb-3">
                                <label for="password" class="form-label">Password</label>
                                <input type="password" class="form-control" id="password" v-model="password" placeholder="Password">
                            </div>
                            <div class="d-grid gap-2">
                                <button type="submit" class="btn btn-primary">Login</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
</template>

<script>
    import axios from 'axios'
    export default {
        name: 'Login',
        data() {
            return {
                username: '',
                password: ''
            };
        },
        methods: {
            async login() {
                await axios.post('api/Security/authenticate', {
                    username: this.username,
                    password: this.password
                }).then((response) => {
                    console.log(response.data.payLoad.token);
                    localStorage.setItem('token', response.data.payLoad.token)
                    localStorage.setItem('UserName', response.data.payLoad.user.UserName)
                    axios.defaults.headers.common['Authorization'] = localStorage.getItem('token');
                    this.$store.dispatch('user', response.data.payLoad.user.UserName);
                    this.$router.push('/');
                });
                
                this.$emit('login', { username: this.username });
                this.username = '';
                this.password = '';
            }
        }
    }
</script>

<style scoped>
 
</style>
