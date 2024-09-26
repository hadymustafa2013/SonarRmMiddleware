<template>
    <header class="header">
        <div class="logo">
            <h1>CreditLens Middleware</h1>
        </div>
        <div v-if="userInfo">Welcome, {{ userInfo.username }}!</div>
        <nav>
            <router-link to="/">Home</router-link> |
            <router-link to="/about">About</router-link>
            <router-link to="/contacts">Contacts</router-link>
        </nav>
            <div class="search-bar">
                <input type="text" placeholder="Search" class="search-input">
                <button class="search-button">Search</button>
            </div>
    </header>
</template>

<script>
    export default {
        name: 'Header'
    };
</script>

<style scoped>

</style>
