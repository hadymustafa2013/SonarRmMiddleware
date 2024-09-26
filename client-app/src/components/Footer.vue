<template>
    <footer class="footer mt-auto py-1 bg-primary">
        <div class="container text-center">
            <p>&copy; {{ currentYear }} RiskMatrix</p>
        </div>
    </footer>
</template>

<script>
    export default {
        name: 'Footer',
        data() {
            return {
                currentYear: new Date().getFullYear()
            };
        }
    };
</script>

<style scoped>
    footer {
        position: fixed;
        bottom: 0;
        width: 100%;
        color:white;
    }
</style>