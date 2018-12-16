<template>
  <v-container>
    <div class="loading" v-if="loading">Loading...</div>
    <div class="loading" v-else-if="error">{{error}}</div>
    <div class="loading" v-else>
      <v-layout text-xs-center wrap>
        <v-flex mb-4>
          <h1 class="display-2 font-weight-bold mb-3">Welcome to Room {{ roomId }}</h1>
        </v-flex>

        <v-flex mb-5 xs12>
          <v-container v-bind="{ [`grid-list-md`]: true }" fluid>
            <v-layout row wrap>
              <v-flex v-for="participant in participants" :key="participant.id" xs4>
                <v-card
                  flat
                  tile
                  height="150px"
                  v-bind:color="participant.isRaised ? 'blue' : ''"
                >{{participant.name}}</v-card>
              </v-flex>
            </v-layout>
          </v-container>
        </v-flex>
      </v-layout>
    </div>
  </v-container>
</template>

<script>
import { apiUrl } from '../services/index.js'

export default {
  computed: {
    roomId: function() {
      return this.$route.params.roomId;
    }
  },
  created () {
    // fetch the data when the view is created and the data is
    // already being observed
    this.fetchData()
  },
  watch: {
    // call again the method if the route changes
    '$route': 'fetchData'
  },
  data: () => ({
    loading: true,
    error: null,
    participants: [
      {
        id: 1,
        name: "dan",
        isRaised: true
      },
      {
        id: 2,
        name: "kelsey",
        isRaised: false
      }
    ]
  }),
  methods: {
    fetchData () {
      this.error = this.participants = null
      this.loading = true;
      fetch(`${apiUrl}api/room/${this.roomId}/participant`, {
            method: "GET",
            mode: "cors",
            cache: "no-cache",
            credentials: "omit", // include, *same-origin, omit
            redirect: "follow", // manual, *follow, error
        }).then(response => {
            if (response.ok) {
                response.json().then(participants => {
                    this.participants = participants
                });
                return;
            }
            response.text().then(errors => {
                this.error = errors;
                console.error(errors);
            });
        }).catch(reason => {
            this.error = reason;
            console.log('Unknown error');
        }).finally(() => this.loading = false);
        
    }
  }
};
</script>

<style>
</style>
