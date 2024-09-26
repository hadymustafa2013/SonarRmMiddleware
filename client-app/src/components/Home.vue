<template>
    <div v-if="user">
        <div class="card shadow rounded-lg">
            <div class="card-header">Dashboard</div>
            <div class="card-body">
                <div class="lead">
                    Welcome, <em>{{ user }}</em>!
                </div>
            </div>
        </div>
    </div>

    <div>
        <h3 v-if="!user">You're not logged in!</h3>
    </div>
    <div>
        <h2>Latest Rating API Response</h2>
        <div v-if="loading" class="loading-icon-container">
            <div class="spinner-border text-primary" role="status">
                <span class="sr-only">Loading...</span>
            </div>
        </div>
        <div v-else>
            <div v-if="error">Error: {{ error }}</div>
            <div>
                <h4>JSON Data:</h4>
                <JsonPretty :data="responseData" :collapsible="true" :showKeyValueSpace="true" :showLine="true" :depth="4" :showIcon="true" :showDoubleQuotes="true" :openAll="false" :syntaxHighlight="true" :colors="{keys: 'blue', strings: 'green', numbers: 'red', booleans: 'purple', nulls: 'gray' }" ></JsonPretty>
            </div>
        </div>
    </div>
    
</template>


<script>
    import axios from "axios";
    import { mapGetters } from 'vuex';
    import JsonPretty from 'vue-json-pretty';
    import 'vue-json-pretty/lib/styles.css';
    export default {
        name: 'Home',
        components: {
            JsonPretty 
        },
        data() {
            return {
                loading: false,
                error: '',
                responseData: null
            };
        },
        computed: {
            ...mapGetters(['user'])
        },
        async created() {
            this.loading = true;
            try {
                await axios.get('api/test/GetEntityInfo').then(response => {
                    this.responseData = JSON.parse(response.data);
                    });

                } catch (error) {
                    this.error = error.message;
                } finally {
                    this.loading = false;
                }
            }
        }
    
</script>

<style scoped>
    .json-view-box {
        border: 1px solid #ccc;
        padding: 10px;
        overflow: auto;
        max-height: 300px; /* Adjust as needed */
    }
</style>