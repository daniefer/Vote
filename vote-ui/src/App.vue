<template>
  <v-app>
    <!-- <v-navigation-drawer
        v-model="drawer"
        fixed
        app
      >
        <v-list dense>
          <v-list-tile>
            <v-list-tile-action>
              <v-icon>home</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>Home</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <v-list-tile>
            <v-list-tile-action>
              <v-icon>contact_mail</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>Contact</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
        </v-list>
    </v-navigation-drawer> -->
    <v-toolbar app>
      <!-- <v-toolbar-side-icon @click.stop="drawer = !drawer"></v-toolbar-side-icon> -->
      <router-link to="/" class="grey--text">
        <v-toolbar-title class="headline">
            <span class="font-weight-light">group</span>
            <span class="text-uppercase">Vote</span>
        </v-toolbar-title>
      </router-link>
      <v-spacer></v-spacer>
      <v-btn
        flat
        to="/room"
      >
        <span class="mr-2">Create Room</span>
      </v-btn>
    </v-toolbar>

    <v-content>
      <router-view/>
    </v-content>
  </v-app>
</template>

<script>

export default {
  name: 'App',
  data: () => ({
      drawer: null
    }),
}
</script>
