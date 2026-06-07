/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: ['./src/**/*.{html,ts}'],
  theme: {
    extend: {
      colors: {
        journal: {
          primary: '#7047e8',
          primaryDark: '#8f6cff',
          danger: '#e71335',
          star: '#f8ad16',
        },
      },
      boxShadow: {
        soft: '0 14px 36px rgba(44, 34, 81, 0.1)',
        'soft-dark': '0 14px 36px rgba(0, 0, 0, 0.32)',
      },
    },
  },
  plugins: [],
};
