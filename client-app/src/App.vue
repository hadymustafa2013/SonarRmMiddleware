<template>
    <Navbar />
    <router-view></router-view>
    <Footer />
</template>

<script>
    import Navbar from './components/Navbar.vue'
    import Footer from './components/Footer.vue'

export default {
  name: 'App',
     data() {
     return {
         user: null
     };
 },

 async created() {
        this.user = localStorage.getItem('UserName');
        this.$store.dispatch('user',localStorage.getItem('UserName'))
     },


  components: {
      Navbar,
      Footer
  }
}
</script>

<style>


</style>
