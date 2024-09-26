<template>

    <div>
        <!-- Decorated Button -->
        <button class="decorated-button" @click="refreshData">
            <span v-if="ShowData">Clear</span>
            <span v-else>Refresh</span>
        </button>

        <!-- Decorated List -->
        <ul class="decorated-list">
            <li v-for="(entity, index) in entities" :key="index">{{ entity.Name }}</li>
        </ul>
    </div>
</template>

<script type="module">
    import axios from 'axios';
    const API_URL = "http://localhost:5079";
    export default {
        name: 'ApiExample',
        data() {
            return {
                title: "Middleware Client",
                ShowData: false,
                entities: []
            };
        },methods: {
            async refreshData() {
                if (this.ShowData === false) {
                    axios.get(API_URL + "/api/Test/GetEntityInfo").then(
                        (response) => {
                            this.entities = response.data;
                            this.ShowData = true;
                        }
                    );
                }
                else {
                    this.clearData();
                }
            },
            clearData() {
                this.entities = [];
                this.ShowData = false;
            }
        }, mounted: function () {
            //this.refreshData();
        }
    };
</script>

<style>
    * Decorated List */
    .decorated-list {
        list-style: none;
        padding: 0;
    }

    .decorated-list li {
        background-color: #f0f0f0;
        padding: 10px;
        margin-bottom: 5px;
        border-radius: 5px;
    }

    /* Decorated Button */
    .decorated-button {
        padding: 10px 20px;
        font-size: 16px;
        background-color: #007bff;
        color: #fff;
        border: none;
        border-radius: 5px;
        cursor: pointer;
        transition: background-color 0.3s ease;
    }

        .decorated-button:hover {
            background-color: #0056b3;
        }
</style>